using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using contracts.Models;
using hamroktmMainClient.Proxies;

namespace hamroktmMainClient.Controllers
{
    [TokenAdminAuthorize]
    public class CategoryController : BaseController
    {
        private readonly ICategoryProxy _proxy;

        public CategoryController(ICategoryProxy proxy)
        {
            _proxy = proxy;
        }   

        // GET: Category
        [AllowAnonymous]
        public async Task<ActionResult> Index(string category = null, string subCategory = null)
        {
            ViewData["category"] = category;
            ViewData["subCategory"] = subCategory;
            if (category != null && subCategory != null)
            {
                category = Uri.EscapeDataString(category);
                subCategory = Uri.EscapeDataString(subCategory);
                var response = await GetAdsBySubCategory(category, subCategory);
                if (response != null)
                {
                    return View(response);
                }
            }
            return View(new List<AdContract>());
        }

        [AllowAnonymous]
        public async Task<IEnumerable<AdContract>> GetAdsBySubCategory(string category, string subCategory)
        {
            var response = await _proxy.GetAdsBySubCategory(category, subCategory);
            return response;

        }

        [HttpPost]
        public async Task<ActionResult> PostCategory(CategoryData categoryModel, List<SubCategoryContract> subCategoryModel)
        {
            var newModel = new CategoryContract()
            {
                Category = categoryModel,
                SubCategories = subCategoryModel
            };

            var token = GetToken();
            var response = await _proxy.PostCategory(newModel, token);
            return new HttpStatusCodeResult(response.StatusCode);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var token = GetToken();
            var response =await _proxy.DeleteCategory(id, token);
            return new HttpStatusCodeResult(response.StatusCode, "Deleted!");
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetPostCategory()
        {
            return PartialView("Partial/Category/PostCategoryPartial");
        }

        //this is for home page. Need to do some partialview update with JS to make it look cool
        [AllowAnonymous]
        public async Task<ActionResult> CategoriesTreePartial()
        {
            var response = await GetAllCategories();
            return PartialView("Partial/Category/CategoriesTreePartial", response);
        }

        //this is for any ajax call to get json object like for getting Category,Subcategory when posting ad
        [AllowAnonymous]
        public async Task<ActionResult> GetCategoriesJson()
        {
            var response = await GetAllCategories();
            //var data=new JavaScriptSerializer().Serialize(response);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //this method gets all the categories and sub from db and returns the category contract model with data.
        [AllowAnonymous]
        public async Task<IEnumerable<CategoryContract>> GetAllCategories()
        {
            var response = await _proxy.GetCategoriesAndSub();
            var categoryContracts = response.OrderBy(x => x.Category.Name);
            return categoryContracts;
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetCategories()
        {
            var response = await GetAllCategories();

            return PartialView("Partial/Category/CategoriesPartial", response);
        }

        //edit category 
        [HttpPut]
        public async Task<ActionResult> EditCategory(CategoryData categoryModel, List<SubCategoryContract> subCategoryModel)
        {
            var newModel = new CategoryContract()
            {
                Category = categoryModel,
                SubCategories = subCategoryModel
            };

            if (ModelState.IsValid)
            {
                var token = GetToken();
                var response = await _proxy.EditCategory(newModel, token);
                return new HttpStatusCodeResult(response.StatusCode, response.ReasonPhrase);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //get category by Id
        [AllowAnonymous]
        public async Task<ActionResult> GetCategoryById(int id)
        {
            var response = await _proxy.GetCategoryById(id);
            return PartialView("Partial/Category/EditCategoryPartial", response);
        }
    }
}