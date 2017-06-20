using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace hamroktmMainClient.Adapter
{
    public interface IProxyAdapter
    {

        Task<HttpResponseMessage> Post(string url, HttpContent content);
        Task<HttpResponseMessage> Post(string url, HttpContent content, string token);
        Task<HttpResponseMessage> Put(string url, HttpContent content, string token);
        Task<HttpResponseMessage> Patch(string url, HttpContent data);
        Task<HttpResponseMessage> Patch(string url, HttpContent data, string token);
        Task<HttpResponseMessage> Delete(string url, string token);
        Task<HttpResponseMessage> Get(string url);
        Task<T> GetAsyncWithToken<T>(string url, string token);
        Task<T> GetAsync<T>(string url);
        Task<T> GetAsync<T>(string url, string data);
    }

    public class ProxyAdapter : IProxyAdapter
    {
        private string BaseUri { get; set; }

        public ProxyAdapter()
        {
            BaseUri = ConfigurationManager.AppSettings["DefaultLocalRoot"];
        }

        //for post without token like register, login....
        public async Task<HttpResponseMessage> Post(string url, HttpContent content)
        {
            try
            {
                var finalUri = BaseUri + url;
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(finalUri, content);
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //for post with token like with permission posting ad...
        public async Task<HttpResponseMessage> Post(string url, HttpContent content, string token)
        {
            try
            {
                var finalUri = BaseUri + url;
                using (var client = new HttpClient())
                {
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    var response = await client.PostAsync(finalUri, content);
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //for put with token
        public async Task<HttpResponseMessage> Put(string url, HttpContent content, string token)
        {
            try
            {
                var finalUri = BaseUri + url;
                using (var client = new HttpClient())
                {
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    var response = await client.PutAsync(finalUri, content);
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //for patch with token
        public async Task<HttpResponseMessage> Patch(string url, HttpContent data)
        {
            try
            {
                var method = new HttpMethod("PATCH");
                var finalUri = BaseUri + url;

                var request = new HttpRequestMessage(method, finalUri)
                {
                    Content = data
                };
                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(request);
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //for patch with token
        public async Task<HttpResponseMessage> Patch(string url, HttpContent data, string token)
        {
            try
            {
                var method = new HttpMethod("PATCH");
                var finalUri = BaseUri + url;

                var request = new HttpRequestMessage(method, finalUri)
                {
                    Content = data
                };
                using (var client = new HttpClient())
                {
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    var response = await client.SendAsync(request);
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //for delete with token
        public async Task<HttpResponseMessage> Delete(string url, string token)
        {
            try
            {
                var finalUri = BaseUri + url;
                using (var client = new HttpClient())
                {
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    var response = await client.DeleteAsync(finalUri);
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> Get(string url)
        {
            try
            {
                var finalUri = BaseUri + url;
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(finalUri);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<HttpResponseMessage>(json);
                    }
                    return new HttpResponseMessage(response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //for get with token.. no content
        public async Task<T> GetAsyncWithToken<T>(string url, string token)
        {
            try
            {
                var finalUri = BaseUri + url;
                using (var client = new HttpClient())
                {
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    var response = await client.GetAsync(finalUri);
                    var json = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //for get without token
        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var finalUri = BaseUri + url;
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(finalUri);
                    var json = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //for get without token - used for some account methods (refactor later)
        public async Task<T> GetAsync<T>(string url, string data)
        {
            try
            {
                var finalUri = BaseUri + url + "?" + data;
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(finalUri);
                    var json = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}