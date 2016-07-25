using System.Web;
using System.Web.Optimization;

namespace Dwim
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                      "~/Scripts/angular.js",
                      "~/Scripts/angular-resource.js",
                      "~/Scripts/angular-sanitize.js",
                      "~/Scripts/angular-ui-layout.js",
                      "~/Scripts/angular-ui-select.js"));

            // App
            bundles.Add(new ScriptBundle("~/bundles/app-common").Include(
                      "~/Scripts/Services/restDataService.js",
                      "~/Scripts/App/app.js"));

            // Chatting
            bundles.Add(new ScriptBundle("~/bundles/chatting").Include(
                      "~/Scripts/Controllers/chatting.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
