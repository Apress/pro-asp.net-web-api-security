using System.Web;
using System.Web.Mvc;

namespace TalentManagerSecured
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // Uncomment the line below, if you want to remove authorization element from web.config
            //filters.Add(new AuthorizeAttribute());
        }
    }
}