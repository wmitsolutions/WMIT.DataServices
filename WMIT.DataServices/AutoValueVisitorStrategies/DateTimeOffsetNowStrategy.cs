using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Common;
using WMIT.DataServices.Visitors;

namespace WMIT.DataServices.AutoValueVisitorStrategies
{
    public class DateTimeOffsetNowStrategy : IAutoValueStrategy
    {
        public object GetValue(EntityContext context)
        {
            return DateTimeOffset.Now;
        }
    }
}
