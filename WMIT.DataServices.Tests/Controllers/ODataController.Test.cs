using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.OData.Results;
using WMIT.DataServices.Controllers;
using WMIT.DataServices.Services;
using WMIT.DataServices.Tests.Fixtures;

namespace WMIT.DataServices.Tests.Controllers
{
    [TestClass]
    public class ODataControllerTests
    {
        #region Initialization

        TestDB db = null;
        EntityDataService<TestDB, Contact> service = null;
        ContactsODataController ctrl = null;

        [TestInitialize]
        public void Initialize()
        {
            db = TestDB.Create();
            
            var user = new User("user");
            service = new EntityDataService<TestDB, Contact>(db, user.Identity);
            ctrl = new ContactsODataController(service);

            // We need the empty configuration for the Validate() method call
            // in the update tests
            ctrl.Configuration = new HttpConfiguration();
        }

        #endregion

        #region Tests - GetAll

        [TestMethod]
        public void GetAll_CanGetAllDataEntries()
        {
            var result = ctrl.Get();
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IQueryable<Contact>>));

            var entries = ((OkNegotiatedContentResult<IQueryable<Contact>>)result).Content;
            Assert.AreEqual(4, entries.ToList().Count);
        }

        #endregion

        #region Tests - GetById

        [TestMethod]
        public void GetById_CanFindSingleEntityById()
        {
            var result = ctrl.Get(1);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<SingleResult<Contact>>));

            var resultContent = ((OkNegotiatedContentResult<SingleResult<Contact>>)result).Content;
            var contact = resultContent.Queryable.Single();

            Assert.AreEqual(1, contact.Id);
            Assert.AreEqual("Terese", contact.FirstName);
            Assert.AreEqual("Redman", contact.LastName);
            Assert.AreEqual(false, contact.IsDeleted);
            Assert.AreEqual("system", contact.CreatedBy);
            Assert.AreEqual(new DateTime(2015, 04, 13, 15, 30, 22), contact.CreatedAt);
        }

        [TestMethod]
        public void GetById_CanReturnNotFound()
        {
            var result = ctrl.Get(999); // No contact 999 in data source
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<SingleResult<Contact>>));

            var resultContent = ((OkNegotiatedContentResult<SingleResult<Contact>>)result).Content;
            // We have to test against an empty SingleResult here, because we can't test against the
            // HTTP status code. This is because we are testing against the controller methods instead of
            // simulating HTTP calls. We should change this to real http calls against a test webserver.
            // TODO: Change controller method tests to http tests
            var foundContacts = resultContent.Queryable.ToList();
            Assert.AreEqual(0, foundContacts.Count);
        }

        [TestMethod]
        public void CanReturnNotFoundForDeletedEntries()
        {
            var result = ctrl.Get(5); // Contact 5 is deleted (IsDeleted = true)
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<SingleResult<Contact>>));

            var resultContent = ((OkNegotiatedContentResult<SingleResult<Contact>>)result).Content;
            var foundContacts = resultContent.Queryable.ToList();
            Assert.AreEqual(0, foundContacts.Count);
        }

        #endregion

        #region Tests - Deletion

        [TestMethod]
        public async Task Delete_CanDeleteEntry()
        {
            var deletionResult = await ctrl.Delete(1);
            Assert.IsInstanceOfType(deletionResult, typeof(StatusCodeResult));

            var deletionResultStatusCode = ((StatusCodeResult)deletionResult).StatusCode;
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, deletionResultStatusCode);

            var contacts = ((OkNegotiatedContentResult<IQueryable<Contact>>)ctrl.Get()).Content.ToList();
            Assert.AreEqual(3, contacts.Count);

            var deletedContactGetResult = ctrl.Get(1);
            Assert.IsInstanceOfType(deletedContactGetResult, typeof(OkNegotiatedContentResult<SingleResult<Contact>>));

            var resultContent = ((OkNegotiatedContentResult<SingleResult<Contact>>)deletedContactGetResult).Content;
            var foundContacts = resultContent.Queryable.ToList();
            Assert.AreEqual(0, foundContacts.Count);
        }

        #endregion

        #region Tests - Insert

        [TestMethod]
        public async Task Insert_CanInsertEntry()
        {
            var contact = new Contact()
            {
                FirstName = "Chuck",
                LastName = "Berry"
            };

            var result = await ctrl.Post(contact);
            Assert.IsInstanceOfType(result, typeof(CreatedODataResult<SingleResult<Contact>>));

            var resultContact = ((CreatedODataResult<SingleResult<Contact>>)result).Entity.Queryable.Single();
            Assert.AreEqual(6, resultContact.Id);
        }

        [TestMethod]
        public async Task Insert_CreationStats()
        {
            var contact = new Contact()
            {
                FirstName = "Chuck",
                LastName = "Berry"
            };

            var time = DateTime.Now;

            var result = ((CreatedODataResult<SingleResult<Contact>>)await ctrl.Post(contact));
            var resultContact = ((CreatedODataResult<SingleResult<Contact>>)result).Entity.Queryable.Single();

            Assert.AreEqual("user", resultContact.CreatedBy);
            Assert.IsTrue((resultContact.CreatedAt - time) < TimeSpan.FromMinutes(5));
        }

        #endregion

        #region Tests - Update

        [TestMethod]
        public async Task Update_CanUpdateEntry()
        {
            var contact = ((OkNegotiatedContentResult<SingleResult<Contact>>)ctrl.Get(1)).Content.Queryable.Single();
            contact.FirstName = "Changeme";

            var result = await ctrl.Put(contact.Id, contact);
            Assert.IsInstanceOfType(result, typeof(UpdatedODataResult<SingleResult<Contact>>));

            var resultContact = ((UpdatedODataResult<SingleResult<Contact>>)result).Entity.Queryable.Single();
            Assert.AreEqual("Changeme", resultContact.FirstName);

            var updatedContact = ((OkNegotiatedContentResult<SingleResult<Contact>>)ctrl.Get(1)).Content.Queryable.Single();
            Assert.AreEqual("Changeme", updatedContact.FirstName);
        }

        [TestMethod]
        public async Task Update_ModificationStats()
        {
            var contact = ((OkNegotiatedContentResult<SingleResult<Contact>>)ctrl.Get(1)).Content.Queryable.Single();
            contact.FirstName = "Changeme";

            var time = DateTime.Now;
            var resultContact = ((UpdatedODataResult<SingleResult<Contact>>)await ctrl.Put(contact.Id, contact)).Entity.Queryable.Single();
            
            Assert.AreEqual("user", resultContact.ModifiedBy);
            Assert.IsTrue((resultContact.ModifiedAt - time) < TimeSpan.FromMinutes(5));
        }

        #endregion
    }
}
