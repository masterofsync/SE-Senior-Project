using System.Threading.Tasks;
using System.Web.Mvc;
using hamroktmMainClient.Proxies;

namespace hamroktmMainClient.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
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

        public ActionResult Unauthorized()
        {
            ViewBag.message = "Not Authorized";

            return View();
        }
    }
}