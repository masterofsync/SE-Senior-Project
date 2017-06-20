using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using contracts.Models;
using hamroktmMainClient.Adapter;
using hamroktmMainClient.Proxies.Models;
using Newtonsoft.Json;

namespace hamroktmMainClient.Proxies
{
    public interface ISendEmailProxy
    {
        Task<HttpResponseMessage> SendForgottenPasswordEmail(AccountBindingContract.SendEmail model);
        Task<HttpResponseMessage> SendConfirmationEmail(AccountBindingContract.SendEmail model);
    }

    public class SendEmailProxy : ISendEmailProxy
    {
        private readonly IProxyAdapter _adapter;

        public SendEmailProxy(IProxyAdapter adapter)
        {
            _adapter = adapter;
        }
        
        public SendEmailProxy()
        {
        }

        public async Task<HttpResponseMessage> SendForgottenPasswordEmail(
            AccountBindingContract.SendEmail model)
        {
            model.Subject = "EasilyBuyAndSell: Reset Password";
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _adapter.Post(ProxyUriList.SendEmail, content);
            return response;
        }

        public async Task<HttpResponseMessage> SendConfirmationEmail(
            AccountBindingContract.SendEmail model)
        {
            model.Subject = "EasilyBuyAndSell: Confirm your account";
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _adapter.Post(ProxyUriList.SendEmail, content);
            return response;
        }
    }
}