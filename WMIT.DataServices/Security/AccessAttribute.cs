using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WMIT.DataServices.Common;

namespace WMIT.DataServices.Security
{
    public enum ViolationBehavior
    {
        IgnoreUserInput = 0,
        Throw = 1
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple=true)]
    public class AccessAttribute : Attribute
    {
        public string Roles { get; set; }
        public string Users { get; set; }

        public EntityOperation On { get; set; }

        /// <summary>
        /// Will lead to an access violation when placed on a property and a user tries to 
        /// write to this property. Used for system fields like CreatedAt, CreatedBy
        /// </summary>
        public bool InternalUsage { get; set; }

        public ViolationBehavior ViolationBehavior { get; set; }
    }
}
