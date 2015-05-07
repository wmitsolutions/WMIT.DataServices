using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using WMIT.DataServices.Common.Attributes;
using WMIT.DataServices.Controllers;

namespace WMIT.DataServices.Common
{
    public static class ModelHelpers
    {
        // TODO: add documentation + tests
        public static void AutoMapODataControllers(this HttpConfiguration configuration, string routeName = "odata", string routePrefix = "odata", Action<ODataConventionModelBuilder> builderConfig = null)
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
                var entitySetConfig = builder.AddEntitySet(entitySetName, entityTypeConfig);

                MapODataControllerActions(builder, ctrlType, entityTypeConfig, entitySetConfig);
                MapODataModelIgnoreProperties(builder, ctrlType, entityTypeConfig);
            }

            if (builderConfig != null)
                builderConfig(builder);

            var model = builder.GetEdmModel();
            configuration.MapODataServiceRoute(
                routeName,
                routePrefix,
                model);
        }

        private static void MapODataControllerActions(ODataConventionModelBuilder builder, Type ctrlType, EntityTypeConfiguration entityTypeConfig, EntitySetConfiguration entitySetConfig)
        {
            // Map actions
            var methods = ctrlType.GetMethods().Where(m => m.GetCustomAttribute<ODataActionAttribute>() != null);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ODataActionAttribute>();
                ProcedureConfiguration procedure;

                Func<string, IEdmTypeConfiguration, ProcedureConfiguration> setBindingParameterFunc;

                if(attr.Type == ODataActionType.Action)
                {
                    procedure = builder.Action(method.Name);
                    setBindingParameterFunc = ((ActionConfiguration)procedure).SetBindingParameter;
                }
                else
                {
                    procedure = builder.Function(method.Name);
                    setBindingParameterFunc = ((FunctionConfiguration)procedure).SetBindingParameter;
                }

                if (attr.Target == ODataActionTarget.Entity)
                {
                    setBindingParameterFunc(BindingParameterConfiguration.DefaultBindingParameterName, entityTypeConfig);
                }
                else
                {
                    var entityCollectionConfig = new CollectionTypeConfiguration(entityTypeConfig, typeof(Enumerable));
                    setBindingParameterFunc(BindingParameterConfiguration.DefaultBindingParameterName, entityCollectionConfig);
                }

                var parameterAttrs = method.GetCustomAttributes<ODataActionParameterAttribute>();
                foreach (var parameterAttr in parameterAttrs)
                {
                    procedure.AddParameter(parameterAttr.Name, builder.GetTypeConfigurationOrNull(parameterAttr.Type));
                }

                procedure.ReturnType = builder.GetTypeConfigurationOrNull(method.ReturnType);
            }
        }

        private static void MapODataModelIgnoreProperties(ODataConventionModelBuilder builder, TypeInfo ctrlType, EntityTypeConfiguration entityTypeConfig)
        {
            var entityType = entityTypeConfig.ClrType;

            foreach(var property in entityType.GetProperties().Where(p => p.GetCustomAttribute<ODataIgnoreAttribute>() != null))
            {
                entityTypeConfig.RemoveProperty(property);
            }
        }
    }
}
