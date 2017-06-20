using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace hamroktmMainClient
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
  "UnpauseAd",
  "Ad/UnpauseAd",
  new { controller = "Ad", action = "UnpauseAd", id = UrlParameter.Optional }
);
            routes.MapRoute(
      "PauseAd",
      "Ad/PauseAd",
      new { controller = "Ad", action = "PauseAd", id = UrlParameter.Optional }
  );
            routes.MapRoute(
                          "GetAdPageResult",
                          "Ad/GetAdPageResult",
                          new { controller = "Ad", action = "GetAdPageResult", id = UrlParameter.Optional }
                      );

            routes.MapRoute(
             "UsersAdList",
             "Ad/UsersAdList",
             new { controller = "Ad", action = "UsersAdList", model = UrlParameter.Optional }
         );

            routes.MapRoute(
      "GetAllAdByUserCount",
      "Ad/GetAllAdByUserCount",
      new { controller = "Ad", action = "GetAllAdByUserCount", userName = UrlParameter.Optional }
  );

            routes.MapRoute(
                "AllPopularAds",
                "Ad/AllPopularAds",
                new { controller = "Ad", action = "AllPopularAds", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                "GetAdCreator",
                "Ad/GetAdCreator",
                new { controller = "Ad", action = "GetAdCreator", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                 "AllRecentAds",
                 "Ad/AllRecentAds",
                 new { controller = "Ad", action = "AllRecentAds", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                  "UpdateViews",
                  "Ad/UpdateViews",
                  new { controller = "Ad", action = "UpdateViews", id = UrlParameter.Optional }
              );

            routes.MapRoute(
                  "GetFeaturedAds",
                  "Ad/GetFeaturedAds",
                  new { controller = "Ad", action = "GetFeaturedAds", id = UrlParameter.Optional }
              );

            routes.MapRoute(
                  "GetRecentAds",
                  "Ad/GetRecentAds",
                  new { controller = "Ad", action = "GetRecentAds", id = UrlParameter.Optional }
              );

            routes.MapRoute(
                  "PopularAds",
                  "Ad/GetPopularAds",
                  new { controller = "Ad", action = "GetPopularAds", id = UrlParameter.Optional }
              );

            routes.MapRoute(
               "ManageAds",
               "Ad/Manage",
               new { controller = "Ad", action = "Manage", id = UrlParameter.Optional }
           );

            routes.MapRoute(
              "PostAd",
              "Ad/PostAd",
              new { controller = "Ad", action = "PostAd", id = UrlParameter.Optional }
          );   
            
            routes.MapRoute(
              "MakeDeal",
              "Ad/MakeDeal",
              new { controller = "Ad", action = "MakeDeal", id = UrlParameter.Optional }
          );

            routes.MapRoute(
            "DeleteAd",
            "Ad/DeleteAd",
            new { controller = "Ad", action = "DeleteAd", id = UrlParameter.Optional }
        );
            routes.MapRoute(
           "GetMyAds",
           "Ad/GetMyAds",
           new { controller = "Ad", action = "GetMyAds", id = UrlParameter.Optional }
       );
            routes.MapRoute(
                      "EditAd",
                      "Ad/EditAd",
                      new { controller = "Ad", action = "EditAd", id = UrlParameter.Optional }
                  );
            routes.MapRoute(
                                 "GetEditAdPartial",
                                 "Ad/GetEditAdPartial",
                                 new { controller = "Ad", action = "GetEditAdPartial", id = UrlParameter.Optional }
                             );
            routes.MapRoute(
          "GetAdById",
          "Ad/GetAdById",
          new { controller = "Ad", action = "GetAdById", id = UrlParameter.Optional }
      );
            routes.MapRoute(
         "RenewAd",
         "Ad/RenewAd",
         new { controller = "Ad", action = "RenewAd", id = UrlParameter.Optional }
     );
            routes.MapRoute("Ad",
                "Ad/{AdId}",
                new { controller = "Ad", action = "Index", AdId = UrlParameter.Optional });

            routes.MapRoute(
             name: "Search",
             url: "Search",
             defaults: new { controller = "Search", action = "Index", q = UrlParameter.Optional }
         );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}
