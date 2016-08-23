using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;


namespace Backup.Web.App_Start
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/JQuery")
                .Include("~/Scripts/jquery-1.9.1.min.js"));

            bundles.Add(new ScriptBundle("~/Scripts/JQueryUnobtrusive")
                .Include("~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Bootstrap")
                .Include("~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Css/Bootstrap")
                .Include("~/Content/bootstrap/bootstrap.min.css",
                         "~/Content/bootstrap/bootstrap-theme.min.css"));
        }
    }
}