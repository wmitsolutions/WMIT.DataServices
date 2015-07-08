using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WMIT.DataServices.Controllers;
using WMIT.DataServices.Services;

namespace WMIT.DataServices.Tests.Fixtures
{
    class ContactsRESTController : RESTController<TestDB, Contact>
    {
        public ContactsRESTController(IDataService<Contact> service) : base(service) { }
        public ContactsRESTController() : base() { }

        public IDataService<Contact> GetService()
        {
            return this.service;
        }

        public static ContactsRESTController Mock() 
        {
            var db = TestDB.Create();

            var user = new User("user");
            var service = new EntityDataService<TestDB, Contact>(db, user);
            var ctrl = new ContactsRESTController(service);

            ctrl.Configuration = new HttpConfiguration();

            return ctrl;
        }
    }

    class ContactsODataController : ODataController<TestDB, Contact>
    {
        public ContactsODataController(IDataService<Contact> service) : base(service) { }
        public ContactsODataController() : base() { }

        public IDataService<Contact> GetService()
        {
            return this.service;
        }

        public static ContactsODataController Mock()
        {
            var db = TestDB.Create();

            var user = new User("user");
            var service = new EntityDataService<TestDB, Contact>(db, user);
            var ctrl = new ContactsODataController(service);

            // We need the empty configuration for the Validate() method call
            // in the update tests
            ctrl.Configuration = new HttpConfiguration();

            return ctrl;
        }
    }
}
