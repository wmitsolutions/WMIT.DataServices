using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Tests.Fixtures
{
    class User : IPrincipal
    {
        public List<string> Roles { get; set; }
        
        private Identity _Identity;
        public IIdentity Identity
        {
            get { return _Identity; }
        }

        public bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }

        public User(string name)
        {
            this.Roles = new List<string>();
            this._Identity = new Identity()
            {
                AuthenticationType = "custom",
                IsAuthenticated = true,
                Name = name
            };
        }
    }

    class Identity : IIdentity
    {
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Name { get; set; }
    }
}
