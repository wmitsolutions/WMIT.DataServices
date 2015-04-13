using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Common
{
    public class NoRowsAffectedException : Exception
    {
        public NoRowsAffectedException() : base("There were no rows affected by the preceding operation.")
        {
        }
    }
}
