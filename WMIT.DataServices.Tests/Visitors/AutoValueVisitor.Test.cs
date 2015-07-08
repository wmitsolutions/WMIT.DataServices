using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Common;
using WMIT.DataServices.Tests.Fixtures;
using WMIT.DataServices.Visitors;

namespace WMIT.DataServices.Tests.Visitors
{
    class OneStrategy : IAutoValueStrategy
    {
        public object GetValue(EntityContext context)
        {
            return 1;
        }
    }

    class TwoStringStrategy : IAutoValueStrategy
    {
        public object GetValue(EntityContext context)
        {
            return "two";
        }
    }


    [TestClass]
    public class AutoValueVisitorTests
    {
        [TestMethod]
        public void AutoValue_Strategies_Insert()
        {
            // Arrange
            var user = new User("my user");

            var db = TestDB.Create();
            var entity = db.AutoValueTestEntities.Attach(new AutoValueTestEntity());
            var entry = db.Entry(entity);
            entry.State = EntityState.Modified;

            var ctx = new EntityContext() { Entry = entry, User = user, Operation = EntityOperation.Insert };

            // Act
            var visitor = new AutoValueVisitor();
            visitor.Visit(ctx);

            // Assert
            Assert.AreEqual(1, entity.AutoOne);
            Assert.AreEqual(null, entity.AutoTwo);
        }

        [TestMethod]
        public void AutoValue_Strategies_Update()
        {
            // Arrange
            var user = new User("my user");

            var db = TestDB.Create();
            var entity = db.AutoValueTestEntities.Attach(new AutoValueTestEntity() { AutoOne = 5 });
            var entry = db.Entry(entity);
            entry.State = EntityState.Modified;

            var ctx = new EntityContext() { Entry = entry, User = user, Operation = EntityOperation.Update };

            // Act
            var visitor = new AutoValueVisitor();
            visitor.Visit(ctx);

            // Assert
            Assert.AreEqual("two", entity.AutoTwo);
            Assert.AreEqual(5, entity.AutoOne);
        }
    }
}
