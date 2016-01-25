using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices.Common
{
    public class DIContainer : IServiceProvider
    {
        private Dictionary<Type, object> mapping = new Dictionary<Type, object>();

        public object GetService(Type serviceType)
        {
            if (mapping.ContainsKey(serviceType))
                return mapping[serviceType];
            else
                return null;
        }

        public void RegisterService(Type serviceType, object service)
        {
            mapping[serviceType] = service;
        }
    }
}
