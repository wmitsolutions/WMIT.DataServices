using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Common;
using WMIT.DataServices.Security;
using WMIT.DataServices.Tests.Fixtures;

namespace WMIT.DataServices.Tests.Security
{
    [TestClass]
    public class AccessVisitorTests
    {
        [TestMethod]
        [ExpectedException(typeof(DataServicesAccessViolationException))]
        public void Access_Internal_Throw()
        {
            // Arrange
            var user = new User("my user");

            var db = TestDB.Create();
            var entity = db.AccessTestEntities.Attach(new AccessTestEntity());
            var entry = db.Entry(entity);

            var ctx = new EntityContext() { Entry = entry, User = user, Operation = EntityOperation.Insert };

            // Access violation
            entry.Property(e => e.Internal_Throw).IsModified = true;

            // Act
            var visitor = new AccessVisitor();
            visitor.Visit(ctx);
        }

        [TestMethod]
        [ExpectedException(typeof(DataServicesAccessViolationException))]
        public void Access_RestrictedUsers_Throw()
        {
            // Arrange
            var user = new User("my user");

            var db = TestDB.Create();
            var entity = db.AccessTestEntities.Attach(new AccessTestEntity());
            var entry = db.Entry(entity);

            var ctx = new EntityContext() { Entry = entry, User = user, Operation = EntityOperation.Insert };

            // Access violation
            entry.Property(e => e.RestrictedUsers_Throw).IsModified = true;

            // Act
            var visitor = new AccessVisitor();
            visitor.Visit(ctx);
        }
      
        [TestMethod]
        [ExpectedException(typeof(DataServicesAccessViolationException))]
        public void Access_RestrictedRoles_Throw()
        {
            // Arrange
            var user = new User("my user");

            var db = TestDB.Create();
            var entity = db.AccessTestEntities.Attach(new AccessTestEntity());
            var entry = db.Entry(entity);

            var ctx = new EntityContext() { Entry = entry, User = user, Operation = EntityOperation.Insert };

            // Access violation
            entry.Property(e => e.RestrictedRoles_Throw).IsModified = true;

            // Act
            var visitor = new AccessVisitor();
            visitor.Visit(ctx);
        }

        [TestMethod]
        public void Access_Internal_Ignore()
        {
            // Arrange
            var user = new User("my user");

            var db = TestDB.Create();
            var entity = db.AccessTestEntities.Attach(new AccessTestEntity());
            var entry = db.Entry(entity);

            var ctx = new EntityContext() { Entry = entry, User = user, Operation = EntityOperation.Insert };

            // Access violation
            entry.Property(e => e.Internal_Ignore).IsModified = true;

            // Act
            var visitor = new AccessVisitor();
            visitor.Visit(ctx);

            // Assert
            Assert.IsFalse(entry.Property(e => e.Internal_Ignore).IsModified);
        }

        [TestMethod]
        public void Access_Granted()
        {
            // Arrange
            var user = new User("admin1");
            user.Roles.Add("admin");

            var db = TestDB.Create();
            var entity = db.AccessTestEntities.Attach(new AccessTestEntity());
            var entry = db.Entry(entity);

            var ctx = new EntityContext() { Entry = entry, User = user, Operation = EntityOperation.Insert };

            // Access violation
            entry.Property(e => e.RestrictedUsers_Throw).IsModified = true;
            entry.Property(e => e.RestrictedRoles_Throw).IsModified = true;

            // Act
            var visitor = new AccessVisitor();
            visitor.Visit(ctx);

            // Assert
            Assert.IsTrue(entry.Property(e => e.RestrictedUsers_Throw).IsModified);
            Assert.IsTrue(entry.Property(e => e.RestrictedRoles_Throw).IsModified);
        }
    }
}
