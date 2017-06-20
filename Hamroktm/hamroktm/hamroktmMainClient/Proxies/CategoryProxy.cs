using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using contracts.Models;
using hamroktmMainClient.Adapter;
using hamroktmMainClient.Proxies.Models;
using Newtonsoft.Json;

namespace hamroktmMainClient.Proxies
{
    public interface ICategoryProxy
    {
        Task<HttpResponseMessage> PostCategory(CategoryContract model, string token);
        Task<HttpResponseMessage> EditCategory(CategoryContract model, string token);
        Task<CategoryContract> GetCategoryById(int id);
        Task<HttpResponseMessage> DeleteCategory(int id, string token);
        Task<IEnumerable<CategoryContract>> GetCategoriesAndSub();
        Task<IEnumerable<AdContract>> GetAdsBySubCategory(string category, string subCategory);
    }

    public class CategoryProxy : ICategoryProxy
    {
        private readonly IProxyAdapter _adapter;

        public CategoryProxy(IProxyAdapter adapter)
        {
            _adapter = adapter;
        }

        //Categories
        public async Task<HttpResponseMessage> PostCategory(CategoryContract model, string token)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _adapter.Post(ProxyUriList.PostCategory, content, token);
            return response;
        }

        //edit Category
        public async Task<HttpResponseMessage> EditCategory(CategoryContract model, string token)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _adapter.Put(ProxyUriList.EditCategoryAndSub, content, token);
            return response;
        }

        //getcategorybyid
        public async Task<CategoryContract> GetCategoryById(int id)
        {
            var content = id.ToString();
            var uri = ProxyUriList.GetCategoriesAndSubById + "?id=" + content;
            var response = await _adapter.GetAsync<CategoryContract>(uri);
            return response;
        }

        public async Task<HttpResponseMessage> DeleteCategory(int id, string token)
        {
            var adapter = new ProxyAdapter();
            var response = await _adapter.Delete(ProxyUriList.DeleteCategory + "?id=" + id, token);
            return response;
        }

        public async Task<IEnumerable<CategoryContract>> GetCategoriesAndSub()
        {
            try
            {
                var response = await _adapter.GetAsync<List<CategoryContract>>(ProxyUriList.GetCategoriesAndSub);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<AdContract>> GetAdsBySubCategory(string category,string subCategory)
        {
            try
            {
                var response = await _adapter.GetAsync<List<AdContract>>(ProxyUriList.GetAdsBySubCategory+"?category="+category+"&subCategory="+subCategory);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}