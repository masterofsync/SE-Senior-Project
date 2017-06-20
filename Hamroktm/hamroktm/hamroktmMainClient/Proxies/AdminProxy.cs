using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using hamroktmMainClient.Adapter;
using hamroktmMainClient.Proxies.Models;

namespace hamroktmMainClient.Proxies
{
    public interface IAdminProxy
    {
        Task<HttpResponseMessage> MakeAdFeatured(int id, string token);
        Task<HttpResponseMessage> RemoveAdFromFeatured(int id, string token);
    }

    public class AdminProxy:IAdminProxy
    {
        private readonly IProxyAdapter _adapter;

        public AdminProxy(IProxyAdapter adapter)
        {
            _adapter = adapter;
        }

        public async Task<HttpResponseMessage> MakeAdFeatured(int id, string token)
        {
            try
            {
                var content = new StringContent(id.ToString(), Encoding.UTF8, "application/json");
                var response = await _adapter.Patch(ProxyUriList.MakeAdFeatured + "?id=" + id, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> RemoveAdFromFeatured(int id, string token)
        {
            try
            {
                var content = new StringContent(id.ToString(), Encoding.UTF8, "application/json");
                var response = await _adapter.Patch(ProxyUriList.RemoveAdFromFeatured + "?id=" + id, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}