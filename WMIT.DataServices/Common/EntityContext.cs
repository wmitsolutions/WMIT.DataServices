using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Common
{
    [Flags]
    public enum EntityOperation
    {
        All = 0,
        Insert = 1 << 0,
        Update = 1 << 1,
        Delete = 1 << 2
    }

    public class EntityContext
    {
        public EntityOperation Operation { get; set; }
        public DbEntityEntry Entry { get; set; }
        public IPrincipal User { get; set; }
    }
}
