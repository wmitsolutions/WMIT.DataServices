using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WMIT.DataServices.Controllers;
using WMIT.DataServices.Demo.Models;

namespace WMIT.DataServices.Demo.Controllers
{
    public class ContactsController : ODataController<AddressBookDB, Contact>
    {

    }
}