using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DoAn1_QuanLyThuVien
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "user",
            //    url: "{area}/{controller}/{action}",
            //    defaults: new { area = "User", controller = "MainUser", action = "Index.cshtml" }

            //);


            //routes.MapRoute(
            //    "User",
            //    "User/{controller}/{action}/{id}",
            //    new { Controller = "MainUser", action = "Index", id = UrlParameter.Optional }
            //    );

            routes.MapRoute(
                name: "user",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "MainUser", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
