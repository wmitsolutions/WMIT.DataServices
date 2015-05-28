using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WMIT.DataServices.Controllers;
using WMIT.DataServices.Services;
using WMIT.DataServices.Tests.Fixtures;

namespace WMIT.DataServices.Tests.Controllers
{
    [TestClass]
    public class ControllersTests
    {
        ContactsODataController odataCtrl;
        ContactsRESTController restCtrl;

        [TestInitialize]
        public void Initialize()
        {
            odataCtrl = ContactsODataController.Mock();
            restCtrl = ContactsRESTController.Mock();
        }

        [TestMethod]
        public void ODataControllers_ShouldBeAbleToAccessServiceInstance()
        {
            var odataService = odataCtrl.GetService();
            Assert.IsNotNull(odataService);

            var restService = restCtrl.GetService();
            Assert.IsNotNull(restService);
        }
    }
}
