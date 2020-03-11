using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace iTelluro.SSO.WebApp
{
    public class Constants
    {
        //Clients 
        public static string ImplicitClient = @"mvc3";
        public static string APIClient = @"MVC Client";

        public static string Authority = ConfigurationManager.AppSettings["AuthorityUrl"];
        public static string RedirectUri = ConfigurationManager.AppSettings["RedirectUrl"];
        public static string PostLogoutRedirectUri = ConfigurationManager.AppSettings["PostLogoutUrl"];
    }

}