using InfoEarthFrame.Web.Next.Filters;
using System.Web;
using System.Web.Mvc;

namespace InfoEarthFrame.Web.Next
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomAuthFilterAttribute());
        }
    }
}