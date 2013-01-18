using System.Web.Optimization;

namespace Taikun.Demo.Web {
  public class BundleConfig {
    public static void RegisterBundles(BundleCollection bundles) {
      bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
        "~/Scripts/lib/jquery-{version}.js",
        "~/Scripts/lib/underscore*",
        "~/Scripts/lib/backbone*",
        "~/Scripts/lib/handlebars*",
        "~/Scripts/lib/jquery.signalR*"
      ));

      bundles.Add(new ScriptBundle("~/bundles/app")
        .IncludeDirectory("~/Scripts/app/models", "*.js", searchSubdirectories: true)
        .IncludeDirectory("~/Scripts/app/collections", "*.js", searchSubdirectories: true)
        .IncludeDirectory("~/Scripts/app/views", "*.js", searchSubdirectories: true)
        .IncludeDirectory("~/Scripts/app/routers", "*.js", searchSubdirectories: true)
        .Include("~/Scripts/app/app*")
      );

      bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

      bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/*.css"));
    }
  }
}