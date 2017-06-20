using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using contracts.Models;
using hamroktmMainServer.Repositories;

namespace hamroktmMainServer.Controllers
{
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/search")]
    public class SearchController : BaseController
    {
        private readonly ISearchRepository _repo;

        public SearchController(ISearchRepository repo)
        {
            _repo = repo;
        }

        // GET: SimpleSearch Get Ads byTags
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("SimpleSearchRequest")]
        public HttpResponseMessage SimpleSearchRequest([FromUri] List<string> searchTags)
        {
            var response = _repo.SimpleSearch(searchTags);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }
       
    }
}