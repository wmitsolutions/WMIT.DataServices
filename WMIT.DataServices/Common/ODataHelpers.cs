using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData.Query;

namespace WMIT.DataServices.Common
{
    public static class ODataHelpers
    {
        public static IQueryable<T> ApplyQuery<T>(this IQueryable<T> query, ODataQueryOptions<T> options)
        {
            return (IQueryable<T>)options.ApplyTo(query);
        }
    }
}
