using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Taikun.Demo.Web {
  public static class WebApiConfig {
    public static void Register(HttpConfiguration config) {
      config.Routes.MapHttpRoute(
        name: "TablesApi",
        routeTemplate: "api/{controller}/{name}/{tableName}"
      );

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{name}",
          defaults: new { name = RouteParameter.Optional }
      );
    }
  }
}
