using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using contracts.Models;
using hamroktmMainClient.Adapter;
using hamroktmMainClient.Models;
using hamroktmMainClient.Proxies.Models;
using Newtonsoft.Json;

namespace hamroktmMainClient.Proxies
{
    public interface IAccountProxy
    {
        Task<HttpResponseMessage> Login(LoginViewModel model);
        Task<HttpResponseMessage> Register(RegisterUserContract model);
        Task<HttpResponseMessage> LogOut(string token);
        Task<HttpResponseMessage> Manage(UserContract model, string token);
        Task<HttpResponseMessage> ChangePassword(AccountBindingContract.ChangePasswordBindingContractModel model, string token);
        Task<HttpResponseMessage> ResetPassword(AccountBindingContract.ResetPasswordContractModel model);
        Task<string> GetForgotPasswordCode(AccountBindingContract.ForgotPasswordContractModel model);
        Task<string> GetConfirmationCode(string email);
        Task<HttpResponseMessage> ConfirmEmail(AccountBindingContract.ConfirmationContractModel model);
        Task<string> GetUserIdByEmail(string email);
        Task<string> GetUserNameByEmail(string email);
        Task<string> GetEmailByUserName(string userName);
        Task<HttpResponseMessage> CheckEmailConfirmation(AccountBindingContract.CheckEmailConfirmation model);
        Task<UserContract> GetUserData(string token);
        Task<UserContract> GetAdPoster(string userName);
        Task<HttpResponseMessage> PatchAccountImage(string imageName, string userName, string token);
        Task<HttpResponseMessage> RemoveProfileImage(string token);
        Task<bool> IsFollowing(string userName, string token);
        Task<HttpResponseMessage> Follow(string userName, string token);
        Task<HttpResponseMessage> Unfollow(string userName, string token);
        Task<List<FollowContract>> GetFollowing(string userName, string token);
        Task<List<FollowContract>> GetFollowers(string userName, string token);
    }

    public class AccountProxy : IAccountProxy
    {
        private readonly IProxyAdapter _adapter;

        public AccountProxy(IProxyAdapter adapter)
        {
            _adapter = adapter;
        } 
        
        public async Task<HttpResponseMessage> Login(LoginViewModel model)
        {
            try
            {
                var content = new StringContent("grant_type=password&username=" + model.UserName + "&password=" + model.Password, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await _adapter.Post(ProxyUriList.LoginUri, content);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> Register(RegisterUserContract model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _adapter.Post(ProxyUriList.RegisterUri, content);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> LogOut(string token)
        {
            try
            {
                HttpContent content = new StringContent("Authorization: Bearer " + token, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await _adapter.Post(ProxyUriList.LogOutUri, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> Manage(UserContract model, string token)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _adapter.Post(ProxyUriList.Manage, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> ChangePassword(
            AccountBindingContract.ChangePasswordBindingContractModel model, string token)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _adapter.Post(ProxyUriList.ChangePassword, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> ResetPassword(
           AccountBindingContract.ResetPasswordContractModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _adapter.Post(ProxyUriList.ResetPassword, content);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetForgotPasswordCode(
            AccountBindingContract.ForgotPasswordContractModel model)
        {
            try
            {
                var content = "email=" + model.Email;
                var response = await _adapter.GetAsync<string>(ProxyUriList.GetForgotPasswordCode, content);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetConfirmationCode(string email)
        {
            try
            {
                var content = "email=" + email;
                var response = await _adapter.GetAsync<string>(ProxyUriList.GetConfirmationCode, content);
                var code = JsonConvert.SerializeObject(response);
                return code;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> ConfirmEmail(AccountBindingContract.ConfirmationContractModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _adapter.Post(ProxyUriList.ConfirmEmail, content);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetUserIdByEmail(string email)
        {
            try
            {
                var content = "email=" + email;
                var response = await _adapter.GetAsync<string>(ProxyUriList.GetUserIdByEmail, content);
                var userId = JsonConvert.SerializeObject(response);
                return userId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetUserNameByEmail(string email)
        {
            try
            {
                var content = "email=" + email;
                var response = await _adapter.GetAsync<string>(ProxyUriList.GetUserNameByEmail, content);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetEmailByUserName(string userName)
        {
            try
            {
                var content = "userName=" + userName;
                var response = await _adapter.GetAsync<string>(ProxyUriList.GetEmailByUserName, content);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpResponseMessage> CheckEmailConfirmation(AccountBindingContract.CheckEmailConfirmation model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _adapter.Post(ProxyUriList.CheckEmailConfirmation, content);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserContract> GetUserData(string token)
        {
            try
            {
                var response = await _adapter.GetAsyncWithToken<UserContract>(ProxyUriList.GetUserData, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserContract> GetAdPoster(string userName)
        {
            try
            {
                var response = await _adapter.GetAsync<UserContract>(ProxyUriList.GetAdPoster + "?userName=" + userName);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //patch user Image
        public async Task<HttpResponseMessage> PatchAccountImage(string imageName, string userName, string token)
        {
            try
            {
                var content = new StringContent(imageName.ToString(), Encoding.UTF8, "application/json");
                var response = await _adapter.Patch(ProxyUriList.PatchAccountImage + "?imageName=" + imageName + "&userName=" + userName, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //remove profile picture
        public async Task<HttpResponseMessage> RemoveProfileImage(string token)
        {
            try
            {
                var response = await _adapter.Delete(ProxyUriList.RemoveProfileImage, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Check if following
        public async Task<bool> IsFollowing(string userName, string token)
        {
            try
            {
                var response = await _adapter.GetAsyncWithToken<bool>(ProxyUriList.IsFollowing + "?userName=" + userName, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Follow User
        public async Task<HttpResponseMessage> Follow(string userName, string token)
        {
            try
            {
                var json = JsonConvert.SerializeObject(userName);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _adapter.Post(ProxyUriList.Follow + "?toFollowUserName=" + userName, content, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Unfollow User
        public async Task<HttpResponseMessage> Unfollow(string userName, string token)
        {
            try
            {
                var response = await _adapter.Delete(ProxyUriList.UnFollow + "?toUnFollowUserName=" + userName, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Get all the following users
        public async Task<List<FollowContract>> GetFollowing(string userName, string token)
        {
            try
            {
                var response = await _adapter.GetAsyncWithToken<List<FollowContract>>(ProxyUriList.GetAllFollowings + "?userName=" + userName, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Get all the users that follow
        public async Task<List<FollowContract>> GetFollowers(string userName, string token)
        {
            try
            {
                var response = await _adapter.GetAsyncWithToken<List<FollowContract>>(ProxyUriList.GetAllFollowers + "?userName=" + userName, token);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}