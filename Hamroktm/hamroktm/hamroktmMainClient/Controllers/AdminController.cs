using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using hamroktmMainClient.Proxies;

namespace hamroktmMainClient.Controllers
{
    [TokenAdminAuthorize]
    public class AdminController : BaseController
    {
        private readonly IAdProxy _adProxy;
        private readonly IAdminProxy _adminProxy;

        public AdminController(IAdProxy proxy, IAdminProxy adminProxy)
        {
            _adProxy = proxy;
            _adminProxy = adminProxy;
        }   

        public ActionResult Index()
        {
            return View();
            //return RedirectToAction("Category");
        }

        public ActionResult GetSearchAdToEditPartial()
        {
            return PartialView("Partial/Admin/SearchAdtoEditPartial");
        }

        //Categories
        public ActionResult Category()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetAdById(int id)
        {
            var response = await _adProxy.GetAdById(id);
            if (response != null)
            {
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPatch]
        public async Task<ActionResult> MakeAdFeatured(int id)
        {
            //if id not found in cookie
            var token = GetToken();
            var response = await _adminProxy.MakeAdFeatured(id, token);
            if (response.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(response.StatusCode, "Added Ad to featured");
            }
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ad already featured!");
            }
            return new HttpStatusCodeResult(response.StatusCode, "Error! Could not make ad featured!");
        }

        [HttpPatch]
        public async Task<ActionResult> RemoveAdFromFeatured(int id)
        {
            //if id not found in cookie
            var token = GetToken();
            var response = await _adminProxy.RemoveAdFromFeatured(id, token);
            if (response.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(response.StatusCode, "Removed Ad from featured!");
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ad not found!");
            }
            return new HttpStatusCodeResult(response.StatusCode, "Error! Could not remove ad from featured!");
        }
    }
}