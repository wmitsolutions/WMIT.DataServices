using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WMIT.DataServices;

namespace WMIT.DataServices.Tests.Common
{
    interface IMyTestInterface
    {
    }
    class MyTestClass : IMyTestInterface {
    }

    [TestClass]
    public class TypeHelperTests
    {
        [TestMethod]
        [TestCategory("TypeHelper")]
        public void CanGetInterfaceFromTypeByType()
        {
            MyTestClass testObject = new MyTestClass();

            Type interfaceType = typeof(MyTestClass).GetInterface<IMyTestInterface>();

            Assert.AreEqual(typeof(IMyTestInterface), interfaceType);
        }

        [TestMethod]
        [TestCategory("TypeHelper")]
        public void CanTestIfInterfaceIsImplementedByType()
        {
            MyTestClass testObject = new MyTestClass();

            bool isImplemented = typeof(MyTestClass).Implements<IMyTestInterface>();

            Assert.AreEqual(true, isImplemented);
        }
    }
}
