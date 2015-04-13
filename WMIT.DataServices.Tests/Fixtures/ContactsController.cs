using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.REST;

namespace WMIT.DataServices.Tests.Fixtures
{
    class ContactsController : RESTController<TestDB, Contact>
    {
        public ContactsController(TestDB db) : base(db) { }
        public ContactsController() : base() { }
    }
}
