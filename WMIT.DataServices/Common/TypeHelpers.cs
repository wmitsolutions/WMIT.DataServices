using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMIT.DataServices
{
    public static class TypeHelpers
    {
        public static bool Implements<TInterface>(this Type type) 
            where TInterface : class
        {
            return type.GetInterface<TInterface>() != null;
        }

        public static Type GetInterface<TInterface>(this Type type) where TInterface : class
        {
            return type.GetInterface(typeof(TInterface).FullName);
        }
    }
}
