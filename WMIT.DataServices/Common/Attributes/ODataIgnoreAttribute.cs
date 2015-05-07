using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ODataIgnoreAttribute : Attribute
    {
    }
}
