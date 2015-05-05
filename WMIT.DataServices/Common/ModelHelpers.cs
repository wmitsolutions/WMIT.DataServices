using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using WMIT.DataServices.Controllers;

namespace WMIT.DataServices.Common
{
    public static class ModelHelpers
    {
        // TODO: add documentation + tests
        public static void AutoMapODataControllers(this HttpConfiguration configuration, string routeName = "odata", string routePrefix = "odata")
        {
            var builder = new ODataConventionModelBuilder();

            // Adds all ODataControllers end their entities to configuration
            var oDataControllerTypes = Assembly.GetCallingAssembly()
                .DefinedTypes
                .Where(t => TypeHelpers.IsSubclassOfRawGeneric(typeof(ODataController<,>), t));

            foreach (var ctrlType in oDataControllerTypes)
            {
                var genericArguments = ctrlType.BaseType.GetGenericArguments();
                var entityType = genericArguments[1];

                var entitySetName = ctrlType.Name.Substring(0, ctrlType.Name.Length - "Controller".Length);
                
                var entityTypeConfig = builder.AddEntityType(entityType);
                builder.AddEntitySet(entitySetName, entityTypeConfig);
            }

            var model = builder.GetEdmModel();
            configuration.MapODataServiceRoute(
                routeName,
                routePrefix,
                model);
        }
    }
}
