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
    public interface ICommentProxy
    {
        Task<HttpResponseMessage> PostComment(AdCommentContract model, string token);
        Task<IEnumerable<AdCommentContract>> GetAllAdComments(int adId);
        Task<HttpResponseMessage> DeleteComment(int commentId, string token);
    }

    public class CommentProxy:ICommentProxy
    {
        private readonly IProxyAdapter _adapter;

        public CommentProxy(IProxyAdapter adapter)
        {
            _adapter = adapter;
        }

        public async Task<HttpResponseMessage> PostComment(AdCommentContract model, string token)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _adapter.Post(ProxyUriList.PostComment, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<AdCommentContract>> GetAllAdComments(int adId)
        {
            try
            {
                var response = await _adapter.GetAsync<List<AdCommentContract>>(ProxyUriList.GetAllAdComment+"?adId="+adId);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> DeleteComment(int commentId,string token)
        {
            try
            {
                var response = await _adapter.Delete(ProxyUriList.DeleteComment + "?commentId=" + commentId, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}