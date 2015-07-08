using Effort;
using Effort.DataLoaders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Common;
using WMIT.DataServices.Model;
using WMIT.DataServices.Security;
using WMIT.DataServices.Tests.Visitors;
using WMIT.DataServices.Visitors;

namespace WMIT.DataServices.Tests
{
    class Contact : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Address> Addresses { get; set; }
    }

    class Address : Entity
    {
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public int ContactId { get; set; }
        public Contact Contact { get; set; }
    }

    class AutoValueTestEntity : Entity
    {
        [AutoValue(On = EntityOperation.Insert, Strategy = typeof(OneStrategy))]
        public int AutoOne { get; set; }

        [AutoValue(On = EntityOperation.Update, Strategy = typeof(TwoStringStrategy))]
        public string AutoTwo { get; set; }
    }

    class AccessTestEntity : Entity
    {
        [Access(InternalUsage = true, ViolationBehavior = ViolationBehavior.Throw)]
        public string Internal_Throw { get; set; }

        [Access(InternalUsage = true, ViolationBehavior = ViolationBehavior.IgnoreUserInput)]
        public string Internal_Ignore { get; set; }

        [Access(Users = "admin1,admin2", ViolationBehavior = ViolationBehavior.Throw)]
        public string RestrictedUsers_Throw { get; set; }

        [Access(Roles = "admin", ViolationBehavior = ViolationBehavior.Throw)]
        public string RestrictedRoles_Throw { get; set; }
    }

    class TestDB : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AccessTestEntity> AccessTestEntities { get; set; }
        public DbSet<AutoValueTestEntity> AutoValueTestEntities { get; set; }

        public TestDB()
        {
        }

        public TestDB(DbConnection connection)
            : base(connection, true)
        {
        }

        static CachingDataLoader cachingDataLoader = null;
        public static TestDB Create()
        {
            if (cachingDataLoader == null)
            {
                var csvDataLoader = new CsvDataLoader("res://WMIT.DataServices.Tests/Fixtures/Data/");
                cachingDataLoader = new CachingDataLoader(csvDataLoader);
            }

            var connection = DbConnectionFactory.CreateTransient(cachingDataLoader);
            var db = new TestDB(connection);

            return db;
        }
    }
}
