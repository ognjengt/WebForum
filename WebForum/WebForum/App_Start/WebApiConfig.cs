using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebForum
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("ActionApi",
              "api/{controller}/{action}",
              null,
              new { action = @"[a-zA-Z]+" });

            config.Routes.MapHttpRoute("WithActionApi",
              "api/{controller}/{id}/{action}");

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
