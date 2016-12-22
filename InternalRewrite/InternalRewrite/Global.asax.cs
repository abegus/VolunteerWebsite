using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace InternalRewrite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute( // this route must be declared first, before the one below it
                 "StartIndex",
                 "EventsAndViews",
                 new
                 {
                     controller = "EventsAndViewsController",
                     action = "StartIndex",
                 });

            routes.MapRoute(
                 "Index",
                 "EventsAndViews/{searchterm}",
                 new
                 {
                     controller = "EventsAndViewsController",
                     action = "Index",
                     searchterm = UrlParameter.Optional
                 });
        }
    }
}
