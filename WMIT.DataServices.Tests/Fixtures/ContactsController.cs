using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Controllers;
using WMIT.DataServices.Services;

namespace WMIT.DataServices.Tests.Fixtures
{
    class ContactsRESTController : RESTController<TestDB, Contact>
    {
        public ContactsRESTController(IDataService<Contact> service) : base(service) { }
        public ContactsRESTController() : base() { }
    }

    class ContactsODataController : ODataController<TestDB, Contact>
    {
        public ContactsODataController(IDataService<Contact> service) : base(service) { }
        public ContactsODataController() : base() { }
    }
}
