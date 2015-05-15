using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ODataProcedureParameterAttribute : Attribute
    {
        public Type Type { get; set; }
        public string Name { get; set; }
    }
}
