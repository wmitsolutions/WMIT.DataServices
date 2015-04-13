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
        [TestCategory("RESTController")]
        public async Task CanGetAllDataEntries()
        {
            var result = await ctrl.GetAll();
            Assert.AreEqual(4, result.Count);
        }

        #endregion

        #region Tests - GetById

        [TestMethod]
        [TestCategory("RESTController")]
        public async Task CanFindSingleEntityById()
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
        [TestCategory("RESTController")]
        public async Task CanReturnNotFound()
        {
            var result = await ctrl.GetEntity(999); // No contact 999 in data source
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        [TestCategory("RESTController")]
        public async Task CanReturnNotFoundForDeletedEntries()
        {
            var result = await ctrl.GetEntity(5); // Contact 5 is deleted (IsDeleted = true)
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        #endregion

        #region Tests - Deletion

        [TestMethod]
        [TestCategory("RESTController")]
        public async Task CanDeleteEntry()
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

        #endregion

        #region Tests - Update

        #endregion
    }
}
