using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Gamers.Web
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

        private void Application_PostResolveRequestCache(object sender, EventArgs e)
        {
            // At this point we've set the handler to take this request, now if this route maps to one
            // we want to redirect to ASP.NET Core, so set the handler to null
            if (Context.Request.Path.Equals("/Home", StringComparison.OrdinalIgnoreCase))
            {
                Context.RemapHandler(null);
            }
        }
    }
}
