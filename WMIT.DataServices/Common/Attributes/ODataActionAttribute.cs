using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Common.Attributes
{
    public enum ODataActionTarget
    {
        Entity,
        Collection
    }

    public enum ODataActionType
    {
        Action,
        Function
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ODataActionAttribute : Attribute
    {
        public string ActionName { get; set; }
        public string NamespaceName { get; set; }
        public ODataActionType Type { get; set; }
        public ODataActionTarget Target { get; set; }
    }
}
