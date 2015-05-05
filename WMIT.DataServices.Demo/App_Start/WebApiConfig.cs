using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Builder;
using WMIT.DataServices.Demo.Models;
using System.Web.OData.Extensions;

namespace WMIT.DataServices.Demo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // OData
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Contact>("Contacts");
            builder.EntitySet<Address>("Addresses");

            //builder.EntityType<Entity>().Ignore(e => e.CreatedAt);
            //builder.EntityType<Entity>().Ignore(e => e.CreatedBy);
            //builder.EntityType<Entity>().Ignore(e => e.ModifiedAt);
            //builder.EntityType<Entity>().Ignore(e => e.ModifiedBy);
            //builder.EntityType<Entity>().Ignore(e => e.IsDeleted);

            config.MapODataServiceRoute(
                routeName: "odata",
                routePrefix: "odata",
                model: builder.GetEdmModel());
        }
    }
}
