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
    public interface IAdProxy
    {
        Task<HttpResponseMessage> PostAd(AdContract model, string token);
        Task<HttpResponseMessage> EditAd(AdContract model, string token);
        Task<HttpResponseMessage> DeleteAd(int id, string token);
        Task<AdContract> GetAdById(int id);
        Task<string> GetAdCreator(int adId);
        Task<IEnumerable<AdContract>> GetMyAds(string token);
        Task<IEnumerable<AdContract>> GetAllAdByUser(string userName);
        Task<IEnumerable<AdContract>> GetPopularAds();
        Task<IEnumerable<AdContract>> GetRecentAds();
        Task<IEnumerable<AdContract>> GetFeaturedAds();
        Task<HttpResponseMessage> UpdateViews(int id);
        Task<HttpResponseMessage> MakeDeal(DealContract model);
        Task<HttpResponseMessage> RenewAd(int id, DateTime endOn, string token);
        Task<HttpResponseMessage> PauseAd(int id, string token);
        Task<HttpResponseMessage> UnpauseAd(int id, string token);
    }

    public class AdProxy:IAdProxy
    {
        private readonly IProxyAdapter _adapter;

        public AdProxy(IProxyAdapter adapter)
        {
            _adapter = adapter;
        }

        public async Task<HttpResponseMessage> PostAd(AdContract model, string token)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _adapter.Post(ProxyUriList.PostAd, content, token);
            return response;
        }

        public async Task<HttpResponseMessage> EditAd(AdContract model, string token)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _adapter.Post(ProxyUriList.EditAd, content, token);
            return response;
        }

        public async Task<HttpResponseMessage> DeleteAd(int id, string token)
        {
            var response = await _adapter.Delete(ProxyUriList.DeleteAd + "?id=" + id, token);
            return response;
        }

        public async Task<AdContract> GetAdById(int id)
        {
            try
            {
                var content = id.ToString();
                var uri = ProxyUriList.GetAdById + "?id=" + content;
                var response = await _adapter.GetAsync<AdContract>(uri);
                return response;
            }
            catch (Exception ex)
            {
                //var responseEx = ((ApiException)ex.InnerException);
                throw new Exception(ex.Message);
            }

        }

        public async Task<string> GetAdCreator(int adId)
        {
            try
            {
                var content = adId.ToString();
                var uri = ProxyUriList.GetAdCreator + "?adId=" + content;
                var response = await _adapter.GetAsync<string>(uri);
                return response;
            }
            catch (Exception ex)
            {
                //var responseEx = ((ApiException)ex.InnerException);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<AdContract>> GetMyAds(string token)
        {
            try
            {
                var response = await _adapter.GetAsyncWithToken<List<AdContract>>(ProxyUriList.GetMyAds, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<AdContract>> GetAllAdByUser(string userName)
        {
            try
            {
                var response = await _adapter.GetAsync<List<AdContract>>(ProxyUriList.GetAllAdByUser+"?userName="+userName);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<AdContract>> GetPopularAds()
        {
            try
            {
                var response = await _adapter.GetAsync<List<AdContract>>(ProxyUriList.GetPopularAds);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<AdContract>> GetRecentAds()
        {
            try
            {
                var response = await _adapter.GetAsync<List<AdContract>>(ProxyUriList.GetRecentAds);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<AdContract>> GetFeaturedAds()
        {
            try
            {
                var response = await _adapter.GetAsync<List<AdContract>>(ProxyUriList.GetFeaturedAds);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> UpdateViews(int id)
        {
            try
            {
                var content = new StringContent(id.ToString(), Encoding.UTF8, "application/json");
                var response = await _adapter.Patch(ProxyUriList.UpdateViews + "?id=" + id, content);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> MakeDeal(DealContract model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _adapter.Post(ProxyUriList.MakeDeal, content);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> RenewAd(int id, DateTime endOn, string token)
        {
            try
            {
                var content = new StringContent(id.ToString(), Encoding.UTF8, "application/json");
                var response = await _adapter.Patch(ProxyUriList.RenewAd + "?id=" + id + "&endOn=" + endOn, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> PauseAd(int id, string token)
        {
            try
            {
                var content = new StringContent(id.ToString(), Encoding.UTF8, "application/json");
                var response = await _adapter.Patch(ProxyUriList.PauseAd + "?id=" + id, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> UnpauseAd(int id, string token)
        {
            try
            {
                var content = new StringContent(id.ToString(), Encoding.UTF8, "application/json");
                var response = await _adapter.Patch(ProxyUriList.UnpauseAd + "?id=" + id, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}