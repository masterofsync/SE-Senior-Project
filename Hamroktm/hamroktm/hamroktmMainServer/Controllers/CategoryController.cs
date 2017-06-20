using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using contracts.Models;
using hamroktmMainServer.Repositories;

namespace hamroktmMainServer.Controllers
{

    [System.Web.Http.Authorize(Roles = "Admin")]
    [System.Web.Http.RoutePrefix("api/Category")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryRepository _repo;

        public CategoryController(ICategoryRepository repo)
        {
            _repo = repo;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("PostCategoryAndSub")]
        public IHttpActionResult PostCategoryAndSub(CategoryContract model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var response = _repo.PostCategoryAndSub(model);

            if (response.StatusCode == 200)
            {
                return Ok();
            }
            return BadRequest();
        }

        //categories
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("EditCategoryAndSub")]
        public IHttpActionResult EditCategoryAndSub(CategoryContract model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var response = _repo.EditCategoryAndSub(model);
            if (response.StatusCode == 200)
            {
                return Ok();
            }
            return BadRequest();
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetCategoriesAndSubById")]
        public HttpResponseMessage GetCategoriesAndSubById(int id)
        {
            var response = _repo.GetCategoriesAndSubById(id);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetCategoriesAndSub")]
        public HttpResponseMessage GetCategoriesAndSub()
        {
            var response = _repo.GetCategoriesAndSub();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // Delete: Ad/DeleteAd
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("DeleteCategory")]
        public IHttpActionResult DeleteCategory(int id)
        {
            var response = _repo.DeleteCategory(id);

            if (response.StatusCode == 200)
            {
                return Ok();
            }
            if (response == new HttpStatusCodeResult(HttpStatusCode.Unauthorized))
            {
                return Unauthorized();
            }
            return BadRequest();
        }

        //REFACTOR THIS WHEN AD TABLE HAS FK OF CATEGORY/SUBCATEGORY TABLE.
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetAdsBySubCategory")]
        public HttpResponseMessage GetAdsBySubCategory(string category, string subCategory)
        {
            var adRepo = new AdRepository();
            var response = adRepo.GetAdsBySubCategory(category, subCategory);

            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}