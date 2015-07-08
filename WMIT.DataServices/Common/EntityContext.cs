using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Common
{
    public class EntityContext
    {
        public DbEntityEntry Entry { get; set; }
        public IPrincipal User { get; set; }
    }
}
