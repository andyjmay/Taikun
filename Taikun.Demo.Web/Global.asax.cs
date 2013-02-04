using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Taikun.SqlServer;


namespace Taikun.Demo.Web {
  public class WebApiApplication : System.Web.HttpApplication {
    protected void Application_Start() {
      RouteTable.Routes.MapHubs();
      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      var builder = new ContainerBuilder();
      builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
      
      //IDatabaseManager<SqlServerDatabase, SqlServerDatabaseTable> dbManager = new SqlServerDatabaseManager(ConfigurationManager.ConnectionStrings["Taikun"].ConnectionString, createIfNotExists: true);
      //builder.RegisterInstance(dbManager).As<IDatabaseManager<IDatabase<IDatabaseTable>, IDatabaseTable>>();

      //IDatabaseManager<IDatabase<IDatabaseTable>, IDatabaseTable> dbManager = new SqlServerDatabaseManager("blah", false);
      
      builder.RegisterInstance(new SqlServerDatabaseManager(ConfigurationManager.ConnectionStrings["Taikun"].ConnectionString, createIfNotExists: true)).As<SqlServerDatabaseManager>();

      var container = builder.Build();
      var resolver = new AutofacWebApiDependencyResolver(container);
      GlobalConfiguration.Configuration.DependencyResolver = resolver;
    }
  }
}