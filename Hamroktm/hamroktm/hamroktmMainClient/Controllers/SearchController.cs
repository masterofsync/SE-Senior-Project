using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using contracts.Models;
using hamroktm.Helpers;
using hamroktmMainClient.Proxies;
using hamroktmMainClient.ViewModels;

namespace hamroktmMainClient.Controllers
{
    public class SearchController : BaseController
    {
        private readonly ISearchProxy _proxy;

        public SearchController(ISearchProxy proxy)
        {
            _proxy = proxy;
        }  
        
        // GET: Search
        public async Task<ActionResult> Index(string q = null)
        {
            ViewData["searchString"] = q;
            if (q != null)
            {
                //replaces all non alphanumeric with space and separates it to an aray
                //split if more than one string and put it in array
                var tool = new RegexUtilities();
                var searchTags = tool.CreateTagsFromText(q);

                //send array to proxy//get all ads
                var response = await _proxy.SimpleSearchRequest(searchTags);
                if (response != null)
                {
                    var finalresult = new SearchViewModel
                    {
                        adContract = response,
                        searchString = q
                    };
                    return View(finalresult);
                }
                return View(new SearchViewModel{adContract = new List<AdContract>(),searchString = q});
            }
            return View(new SearchViewModel { searchString = q });
        }

        [HttpPost]
        public ActionResult GetPageResult(List<AdContract> model = null)
        {
            return PartialView("Partial/Ad/AdListingsPartial", model);
        }
    }
}