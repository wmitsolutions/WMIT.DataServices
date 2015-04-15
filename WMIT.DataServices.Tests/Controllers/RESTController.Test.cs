using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using WMIT.DataServices.REST;
using WMIT.DataServices.Tests.Fixtures;

namespace WMIT.DataServices.Tests.Controllers
{
    [TestClass]
    public class RESTControllerTests
    {
        #region Initialization

        TestDB db = null;
        ContactsController ctrl = null;

        [TestInitialize]
        public void Initialize()
        {
            db = TestDB.Create();
            ctrl = new ContactsController(db);
        }

        #endregion

        #region Tests - GetAll

        [TestMethod]
        public async Task GetAll_CanGetAllDataEntries()
        {
            var result = await ctrl.GetAll();
            Assert.AreEqual(4, result.Count);
        }

        #endregion

        #region Tests - GetById

        [TestMethod]
        public async Task GetById_CanFindSingleEntityById()
        {
            var result = await ctrl.GetEntity(1);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Contact>));

            var contact = ((OkNegotiatedContentResult<Contact>)result).Content;

            Assert.AreEqual(1, contact.Id);
            Assert.AreEqual("Terese", contact.FirstName);
            Assert.AreEqual("Redman", contact.LastName);
            Assert.AreEqual(false, contact.IsDeleted);
            Assert.AreEqual("system", contact.CreatedBy);
            Assert.AreEqual(new DateTime(2015, 04, 13, 15, 30, 22), contact.CreatedAt);
        }

        [TestMethod]
        public async Task GetById_CanReturnNotFound()
        {
            var result = await ctrl.GetEntity(999); // No contact 999 in data source
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CanReturnNotFoundForDeletedEntries()
        {
            var result = await ctrl.GetEntity(5); // Contact 5 is deleted (IsDeleted = true)
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region Tests - Deletion

        [TestMethod]
        public async Task Delete_CanDeleteEntry()
        {
            var deletionResult = await ctrl.DeleteEntity(1);
            Assert.IsInstanceOfType(deletionResult, typeof(OkNegotiatedContentResult<Contact>));

            var deletionResultContact = ((OkNegotiatedContentResult<Contact>)deletionResult).Content;
            Assert.AreEqual(true, deletionResultContact.IsDeleted);

            var contacts = await ctrl.GetAll();
            Assert.AreEqual(3, contacts.Count);

            var deletedContact = await ctrl.GetEntity(1);
            Assert.IsInstanceOfType(deletedContact, typeof(NotFoundResult));
        }

        #endregion

        #region Tests - Insert

        [TestMethod]
        public async Task Insert_CanInsertEntry()
        {
            var contact = new Contact()
            {
                FirstName = "Chuck",
                LastName ="Berry"
            };

            var result = await ctrl.PostEntity(contact);
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<Contact>));

            var resultContact = ((CreatedAtRouteNegotiatedContentResult<Contact>)result).Content;
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

            ctrl.User = new User("user");

            var time = DateTime.Now;
            var resultContact = ((CreatedAtRouteNegotiatedContentResult<Contact>)await ctrl.PostEntity(contact)).Content;
            
            Assert.AreEqual("user", resultContact.CreatedBy);
            Assert.IsTrue((resultContact.CreatedAt - time) < TimeSpan.FromMinutes(5));
        }

        #endregion

        #region Tests - Update

        [TestMethod]
        public async Task Update_CanUpdateEntry()
        {
            var contact = ((OkNegotiatedContentResult<Contact>)await ctrl.GetEntity(1)).Content;
            contact.FirstName = "Changeme";

            var result = await ctrl.PutEntity(contact.Id, contact);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Contact>));

            var resultContact = ((OkNegotiatedContentResult<Contact>)result).Content;
            Assert.AreEqual("Changeme", resultContact.FirstName);

            var updatedContact = ((OkNegotiatedContentResult<Contact>)await ctrl.GetEntity(1)).Content;
            Assert.AreEqual("Changeme", updatedContact.FirstName);
        }

        [TestMethod]
        public async Task Update_ModificationStats()
        {
            var contact = ((OkNegotiatedContentResult<Contact>)await ctrl.GetEntity(1)).Content;
            contact.FirstName = "Changeme";

            ctrl.User = new User("user");

            var time = DateTime.Now;
            var resultContact = ((OkNegotiatedContentResult<Contact>)await ctrl.PutEntity(contact.Id, contact)).Content;

            Assert.AreEqual("user", resultContact.ModifiedBy);
            Assert.IsTrue((resultContact.ModifiedAt - time) < TimeSpan.FromMinutes(5));
        }

        #endregion
    }
}
