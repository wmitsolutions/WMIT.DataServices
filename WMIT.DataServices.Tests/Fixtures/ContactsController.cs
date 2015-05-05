using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Controllers;

namespace WMIT.DataServices.Tests.Fixtures
{
    class ContactsRESTController : RESTController<TestDB, Contact>
    {
        public ContactsRESTController(TestDB db) : base(db) { }
        public ContactsRESTController() : base() { }
    }

    class ContactsODataController : ODataController<TestDB, Contact>
    {
        public ContactsODataController(TestDB db) : base(db) { }
        public ContactsODataController() : base() { }
    }
}
