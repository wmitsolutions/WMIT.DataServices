using System;
using System.Collections;
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
            var methods = ctrlType.GetMethods().Where(m => m.GetCustomAttribute<ODataProcedureAttribute>() != null);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ODataProcedureAttribute>();
                ProcedureConfiguration procedure;

                // TODO: Extract into separate class if possible
                Func<string, IEdmTypeConfiguration, ProcedureConfiguration> setBindingParameterFunc;

                string previousNamespace = builder.Namespace;
                builder.Namespace = attr.Namespace;

                if (attr.Type == ODataProcedureType.Action)
                {
                    // Create an action
                    procedure = builder.Action(attr.ProcedureName ?? method.Name);
                    setBindingParameterFunc = ((ActionConfiguration)procedure).SetBindingParameter;
                }
                else
                {
                    // Create a function
                    procedure = builder.Function(attr.ProcedureName ?? method.Name);
                    setBindingParameterFunc = ((FunctionConfiguration)procedure).SetBindingParameter;
                }

                builder.Namespace = previousNamespace;

                if (attr.Target == ODataProcedureTarget.Entity)
                {
                    // Bind to entity
                    setBindingParameterFunc(BindingParameterConfiguration.DefaultBindingParameterName, entityTypeConfig);
                }
                else
                {
                    // Bind to collection
                    var entityCollectionConfig = new CollectionTypeConfiguration(entityTypeConfig, typeof(Enumerable));
                    setBindingParameterFunc(BindingParameterConfiguration.DefaultBindingParameterName, entityCollectionConfig);
                }

                var parameterAttrs = method.GetCustomAttributes<ODataProcedureParameterAttribute>();
                foreach (var parameterAttr in parameterAttrs)
                {
                    var parameterType = parameterAttr.Type;

                    // Is the parameter type a collection?
                    if (parameterType.IsGenericType && parameterType.GetInterface(typeof(IEnumerable).Name, false) != null)
                    {
                        // If the parameter is a collection, we need to call the generic CollectionParameter<T>(name) method
                        var genericArgument = parameterType.GetGenericArguments().Single();

                        var parameterMethod = procedure.GetType().GetMethod("CollectionParameter");
                        var methodInfo = parameterMethod.MakeGenericMethod(genericArgument);
                        methodInfo.Invoke(procedure, new object[] { parameterAttr.Name });
                    }
                    else
                    {
                        // If the parameter is not a collection, we call the generic Parameter<T>(name) method
                        var parameterMethod = procedure.GetType().GetMethod("Parameter");
                        var methodInfo = parameterMethod.MakeGenericMethod(parameterType);
                        methodInfo.Invoke(procedure, new object[] { parameterAttr.Name });
                    }
                }

                if (attr.Returns != null)
                {
                    var returns = procedure.GetType().GetMethod("Returns");
                    var generic = returns.MakeGenericMethod(attr.Returns);
                    generic.Invoke(procedure, null);
                }
                else if (attr.ReturnsCollection != null)
                {
                    var returnsCollection = procedure.GetType().GetMethod("ReturnsCollection");
                    var generic = returnsCollection.MakeGenericMethod(attr.ReturnsCollection);
                    generic.Invoke(procedure, null);
                }
                else if (attr.ReturnsEntity != null)
                {
                    var returnsEntity = procedure.GetType().GetMethod("ReturnsFromEntitySet", new Type[] { typeof(string) });
                    var generic = returnsEntity.MakeGenericMethod(attr.ReturnsEntity);

                    var entitySet = builder.EntitySets.Single(set => set.ClrType == attr.ReturnsEntity);
                    generic.Invoke(procedure, new object[] { entitySet.Name });
                }
                else if (attr.ReturnsEntityCollection != null)
                {
                    var returnsEntity = procedure.GetType().GetMethod("ReturnsCollectionFromEntitySet", new Type[] { typeof(string) });
                    var generic = returnsEntity.MakeGenericMethod(attr.ReturnsEntityCollection);

                    var entitySet = builder.EntitySets.Single(set => set.ClrType == attr.ReturnsEntityCollection);
                    generic.Invoke(procedure, new object[] { entitySet.Name });
                }
                else
                {
                    procedure.ReturnType = builder.GetTypeConfigurationOrNull(method.ReturnType);
                }
            }
        }

        private static void MapODataModelIgnoreProperties(ODataConventionModelBuilder builder, TypeInfo ctrlType, EntityTypeConfiguration entityTypeConfig)
        {
            var entityType = entityTypeConfig.ClrType;

            foreach (var property in entityType.GetProperties().Where(p => p.GetCustomAttribute<ODataIgnoreAttribute>() != null))
            {
                entityTypeConfig.RemoveProperty(property);
            }
        }
    }
}
