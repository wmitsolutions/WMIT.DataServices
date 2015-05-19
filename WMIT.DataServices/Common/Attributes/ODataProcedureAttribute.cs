using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Common.Attributes
{
    public enum ODataProcedureTarget
    {
        Entity,
        Collection
    }
    public enum ODataProcedureType
    {
        Action,
        Function
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ODataProcedureAttribute : Attribute
    {
        public string ProcedureName { get; set; }
        public string Namespace { get; set; }

        public ODataProcedureAttribute()
        {
            Namespace = "Default";
        }

        public ODataProcedureType Type { get; set; }
        public ODataProcedureTarget Target { get; set; }

        public Type Returns { get; set; }
        public Type ReturnsCollection { get; set; }
        public Type ReturnsEntity { get; set; }
        public Type ReturnsEntityCollection { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ODataActionAttribute : ODataProcedureAttribute
    {
        public ODataActionAttribute()
        {
            this.Type = ODataProcedureType.Action;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ODataFunctionAttribute : ODataProcedureAttribute
    {
        public ODataFunctionAttribute()
        {
            this.Type = ODataProcedureType.Function;
        }
    }
}
