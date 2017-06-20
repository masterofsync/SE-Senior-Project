using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using contracts.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using hamroktmMainServer.Models;
using hamroktmMainServer.Providers;
using hamroktmMainServer.Repositories;
using hamroktmMainServer.Results;

namespace hamroktmMainServer.Controllers
{
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/Account")]
    public class AccountController : BaseController
    {
        private const string LocalLoginProvider = "Local";

        private readonly IAccountRepository _repo;

        public AccountController(IAccountRepository repository)
        {
            _repo = repository;
        }


        // GET api/Account/GetUserIdByEmail
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("GetUserIdByEmail")]
        public async Task<HttpResponseMessage> GetUserIdByEmail(string email)
        {
            ApplicationUser user = await UserManager.FindByEmailAsync(email);
            if (user != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, user.Id);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        // GET api/Account/GetUserIdByEmail
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("GetUserNameByEmail")]
        public async Task<HttpResponseMessage> GetUserNameByEmail(string email)
        {
            ApplicationUser user = await UserManager.FindByEmailAsync(email);
            if (user != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, user.UserName);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("GetEmailByUserName")]
        public async Task<HttpResponseMessage> GetEmailByUserName(string userName)
        {
            ApplicationUser user = await UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, user.Email);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }


        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // GET api/Account/UserInfo
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("GetAdPoster")]
        public HttpResponseMessage GetAdPoster(string userName)
        {
            var adPoster = _repo.GetAdPoster(userName);
            var user = UserManager.FindByName(userName);
            var userId = user.Id;
            if (adPoster != null)
            {
                adPoster.FollowersCount = _repo.GetFollowersCount(userId);
                adPoster.Followingcount = _repo.GetFollowingCount(userId);
                return Request.CreateResponse(HttpStatusCode.OK, adPoster);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // GET api/Account/UserData
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("GetUserData")]
        public async Task<HttpResponseMessage> GetUserData()
        {
            //ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            var userId = RequestContext.Principal.Identity.GetUserId();
            if (userId != null)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var userContract = new UserContract()
                    {
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Id = user.Id,
                        UserName = user.UserName,
                        Phone = user.PhoneNumber,
                        Image = user.Image,
                        FollowersCount = _repo.GetFollowersCount(userId),
                        Followingcount = _repo.GetFollowingCount(userId),
                        CreatedOn = user.CreatedOn
                    };

                    var roles = await UserManager.GetRolesAsync(userId);

                    foreach (var role in roles)
                    {
                        userContract.Role.Add(role.ToString());
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, userContract);
                }
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        // POST api/Account/Logout
        [System.Web.Http.Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        //POST api/Account/ConfirmEmail
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(AccountBindingContract.ConfirmationContractModel model)
        {
            var result = await UserManager.ConfirmEmailAsync(model.UserId, model.Code);

            if (result.Succeeded)
                return Ok();
            return BadRequest(result.Errors.ToString());
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [System.Web.Http.Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }
        //PUT api/Account/SendEmailForgottenPassword
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("SendEmail")]
        public async Task<IHttpActionResult> SendEmail(AccountBindingContract.SendEmail model)
        {
            if (model != null)
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(model.Email);
                mail.From = new MailAddress("easilybuyandsell@gmail.com");
                mail.Subject = model.Subject;
                string Body = model.Content;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential
                    ("easilybuyandsell@gmail.com", "mynameisbikesh"); // Enter seders User name and password
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return Ok();
            }
            return BadRequest("Model is null!");
        }

        [System.Web.Http.Route("ManageProfile")]
        public async Task<IHttpActionResult> ManageProfile(UserContract model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.PhoneNumber = model.Phone;
                    user.UpdatedOn = DateTime.Now;
                }
                var result = await UserManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("PatchAccountImage")]
        public async Task<IHttpActionResult> PatchAccountImage(string imageName, string userName)
        {
            var user = RequestContext.Principal.Identity.GetUserName();
            if (user.Equals(userName,StringComparison.InvariantCultureIgnoreCase))
            {
                var userDB = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (userDB != null)
                {
                    userDB.Image = imageName;
                    userDB.UpdatedOn = DateTime.Now;
                }
                var result = await UserManager.UpdateAsync(userDB);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                return Ok();
            }
            return Unauthorized();
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("RemoveProfileImage")]
        public async Task<IHttpActionResult> RemoveProfileImage()
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            var userDB = await UserManager.FindByIdAsync(userId);
            if (userDB != null)
            {
                userDB.Image = null;
                userDB.UpdatedOn = DateTime.Now;
            }
            var result = await UserManager.UpdateAsync(userDB);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("IsFollowing")]
        public bool IsFollowing(string userName)
        {
            var followerId = RequestContext.Principal.Identity.GetUserId();
            var followingUser = UserManager.FindByName(userName);
            if (followingUser != null)
            {
                var followingId = followingUser.Id;
                var response = _repo.IsFollowing(followerId, followingId);
                return response;
            }
            return false;
        } 
        
        [System.Web.Http.Route("GetAllFollowings")]
        public HttpResponseMessage GetAllFollowings(string userName)
        {
            var userId = UserManager.FindByName(userName).Id;
            var response = _repo.GetAllFollowings(userId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetAllFollowers")]
        public List<FollowContract> GetAllFollowers(string userName)
        {
            var user = UserManager.FindByName(userName);
            var userId=user.Id;
            var response = _repo.GetAllFollowers(userId);
            return response;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Follow")]
        public async Task<IHttpActionResult> Follow(string toFollowUserName)
        {
            var toFollowUser=await UserManager.FindByNameAsync(toFollowUserName);

            var userId = RequestContext.Principal.Identity.GetUserId();
            var response = _repo.Follow(userId, toFollowUser.Id);
            if (response != null)
            {
                if (response.StatusCode == 200)
                {
                    return Ok();
                }
                return BadRequest();
            }
            return BadRequest();
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("UnFollow")]
        public async Task<IHttpActionResult> UnFollow(string toUnFollowUserName)
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            var toUnFollowUser = await UserManager.FindByNameAsync(toUnFollowUserName);
            var response = _repo.UnFollow(userId, toUnFollowUser.Id);
            if (response != null)
            {
                if (response.StatusCode == 200)
                {
                    return Ok();
                }
                return BadRequest();
            }
            return BadRequest();
        }

        // POST api/Account/CheckEmailConfimation
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("CheckEmailConfirmation")]
        public async Task<HttpResponseMessage> CheckEmailConfirmation(AccountBindingContract.CheckEmailConfirmation model)
        {
            var user = await UserManager.FindByNameAsync(model.UserName);
            var result = await UserManager.IsEmailConfirmedAsync(user.Id);

            if (!result)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        // POST api/Account/ChangePassword

        [System.Web.Http.Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(AccountBindingContract.ChangePasswordBindingContractModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [System.Web.Http.AllowAnonymous]
        //POST api/Account/GetForgotPasswordCode
        [System.Web.Http.Route("GetForgotPasswordCode")]
        public async Task<HttpResponseMessage> GetForgotPasswordCode(string email)
        {
            var model = new AccountBindingContract.ForgotPasswordContractModel();
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    model.Code = HttpUtility.UrlEncode(code);
                    model.Email = email;
                    return Request.CreateResponse(HttpStatusCode.OK, code);
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(BadRequest(ModelState));
        }

        [System.Web.Http.AllowAnonymous]
        //POST api/Account/GetForgotPasswordCode
        [System.Web.Http.Route("GetConfirmationCode")]
        public async Task<HttpResponseMessage> GetConfirmationCode(string email)
        {
            var model = new AccountBindingContract.ForgotPasswordContractModel();
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindByEmailAsync(email);

                if (user.Email != null)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    model.Code = HttpUtility.UrlEncode(code);
                    model.Email = email;
                    return Request.CreateResponse(HttpStatusCode.OK, code);
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            return Request.CreateResponse(BadRequest(ModelState));
        }

        //POST api/Account/ResetPassword
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(AccountBindingContract.ResetPasswordContractModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserId != null)
                {

                    var result = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.Password);
                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }
                    return Ok();
                }
                return NotFound();
            }
            return BadRequest(ModelState);
        }

        // POST api/Account/SetPassword
        [System.Web.Http.Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [System.Web.Http.Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [System.Web.Http.Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [System.Web.Http.OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }
            return logins;
        }

        // POST api/Account/Register
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterUserContract model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [System.Web.Http.OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
