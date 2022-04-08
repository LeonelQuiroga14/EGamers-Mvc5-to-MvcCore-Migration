using Gamers.Web.Interfaces;
#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif
namespace Gamers.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContext _httpContext;
        public HomeController()
        {

        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}