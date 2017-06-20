using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using contracts.Models;
using hamroktmMainClient.Adapter;
using hamroktmMainClient.Proxies.Models;

namespace hamroktmMainClient.Proxies
{
    public interface ISearchProxy
    {
        Task<List<AdContract>> SimpleSearchRequest(List<string> searchTags);
    }

    public class SearchProxy : ISearchProxy
    {
        private readonly IProxyAdapter _adapter;
        public SearchProxy(IProxyAdapter adapter)
        {
            _adapter = adapter;
        }

        public async Task<List<AdContract>> SimpleSearchRequest(List<string> searchTags)
        {
            var userSubmittedTags = String.Join("&searchTags=", searchTags);
            var content = "?searchTags=" + userSubmittedTags;
            var response = await _adapter.GetAsync<List<AdContract>>(ProxyUriList.SimpleSearchRequest + content);
            return response;
        }
    }
}