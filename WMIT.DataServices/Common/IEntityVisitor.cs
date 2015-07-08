using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Common
{
    public interface IEntityVisitor
    {
        void Visit(EntityContext context);
    }
}
