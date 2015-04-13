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
using WMIT.DataServices.Model;

namespace WMIT.DataServices.Tests
{
    class Contact : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    class TestDB : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        public TestDB()
        {
        }

        public TestDB(DbConnection connection) : base(connection, true)
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
