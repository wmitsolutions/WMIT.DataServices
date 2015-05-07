using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.OData;
using WMIT.DataServices.Common;
using WMIT.DataServices.Common.Attributes;
using WMIT.DataServices.Controllers;
using WMIT.DataServices.Demo.Models;

namespace WMIT.DataServices.Demo.Controllers
{
    public class ContactsController : ODataController<AddressBookDB, Contact>
    {
        [ODataAction(Target=ODataActionTarget.Collection, Type=ODataActionType.Function)]
        [ODataActionParameter(Name="test", Type=typeof(int))]
        public int AddTag(ODataActionParameters parameters)
        {
            return 5;
        }
    }
}