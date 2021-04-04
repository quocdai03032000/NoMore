using System.Web;
using System.Web.Mvc;

namespace testTHEm_XOA_SUA
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
