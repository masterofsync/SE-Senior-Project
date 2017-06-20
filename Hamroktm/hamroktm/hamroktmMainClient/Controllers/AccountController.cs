using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.WebSockets;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using hamroktmMainClient.Models;
using contracts.Models;
using hamroktmMainClient.Adapter.Model;
using hamroktmMainClient.Proxies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using hamroktm.Helpers;
using hamroktmMainClient.Adapter;
using hamroktmMainClient.ViewModels;

namespace hamroktmMainClient.Controllers
{
    [TokenAuthorize]
    public class AccountController : BaseController
    {
        //private ApplicationSignInManager _signInManager;
        //private ApplicationUserManager _userManager;
        private readonly IAccountProxy _proxy;
        private readonly ISendEmailProxy _sendEmailProxy;

   

        public AccountController(AccountProxy proxy, ISendEmailProxy sendEmailProxy)
        {
            _proxy = proxy;
            _sendEmailProxy = sendEmailProxy;
        }  
        //public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set
        //    {
        //        _signInManager = value;
        //    }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (GetToken() == null)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            return RedirectToLocal(returnUrl);
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                await LogOffTask();
                var util = new RegexUtilities();
                var checkIfEmail = util.IsValidEmail(model.UserName);
                if (checkIfEmail)
                {
                    //if the user enters email, getting username from server.
                    var userName = await _proxy.GetUserNameByEmail(model.UserName);
                    if (userName != null)
                    {
                        model.UserName = userName;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password");
                        return View(model);
                    }
                }

                var response = await _proxy.Login(model);

                if (response != null)
                {
                    var tokenResponse = await response.Content.ReadAsAsync<UserTokenData>(new[] { new JsonMediaTypeFormatter() });

                    if (tokenResponse.AccessToken != null)
                    {
                        var checkConfirm = await _proxy.CheckEmailConfirmation(new AccountBindingContract.CheckEmailConfirmation
                                {
                                    UserName = model.UserName
                                });

                        if (checkConfirm.StatusCode == HttpStatusCode.OK)
                        {
                            var token = tokenResponse.AccessToken;
                            //var expires = tokenResponse.Expires;
                            var expires = DateTime.Now.AddDays(10);

                            HttpCookie tokenCookie = new HttpCookie("ebsCookie");
                            tokenCookie.Values.Add("AccessToken", token);

                            tokenCookie.Values.Add("Username", model.UserName);

                            var roleChecker = await _proxy.GetUserData(token);

                            if (roleChecker.Role.Any())
                            {
                                tokenCookie.Values.Add("Role", roleChecker.Role[0]);
                            }
                            tokenCookie.Expires = expires;
                            tokenCookie.HttpOnly = true;
                            Response.Cookies.Add(tokenCookie);

                            //System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie("AccessToken")
                            //{
                            //    Value = token,
                            //    HttpOnly = true,
                            //    Expires = expires
                            //});
                            //System.Web.HttpContext.Current.Session["AccessToken"] = token;

                            //await
                            //    SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe,
                            //        shouldLockout: false);
                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            if (checkIfEmail)
                            {
                                ViewData["Email"] = model.UserName;
                            }
                            else
                            {
                                var getEmail = await _proxy.GetEmailByUserName(model.UserName);
                                ViewData["Email"] = getEmail;
                            }
                            return View("DisplayEmail");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password");
                        return View(model);
                    }
                    //use this later so that client doesn't have direct connection to database.
                    //var claims = new List<Claim>();
                    //claims.Add(new Claim(ClaimTypes.Name, model.UserName));

                    //var id= new ClaimsIdentity(claims,DefaultAuthenticationTypes.ApplicationCookie);

                    //var ctx = Request.GetOwinContext();
                    //var authenticationmanager = ctx.Authentication;
                    //authenticationmanager.SignIn(id);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);


            //// This doesn't count login failures towards account lockout
            //// To enable password failures to trigger account lockout, change to shouldLockout: true
            //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        return RedirectToLocal(returnUrl);
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            //    case SignInStatus.Failure:
            //    default:
            //        ModelState.AddModelError("", "Invalid login attempt.");
            //        return View(model);
            //}
        }

        public async Task<ActionResult> DashBoard()
        {
            var token = GetToken();

            var userContract = await _proxy.GetUserData(token);

            return PartialView("Partial/PanelDashboardPartial", userContract);
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetAdPoster(string userName)
        {
            var posterData = await _proxy.GetAdPoster(userName);
            return PartialView("Partial/Account/AdPosterPartial", posterData);
        }

        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes. 
        //    // If a user enters incorrect codes for a specified amount of time then the user account 
        //    // will be locked out for a specified amount of time. 
        //    // You can configure the account lockout settings in IdentityConfig
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}


        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> SendConfirmationEmail(string email)
        {
            var newCode = await _proxy.GetConfirmationCode(email);
            var userId = await _proxy.GetUserIdByEmail(email);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = newCode }, protocol: Request.Url.Scheme);
            var newModel = new AccountBindingContract.SendEmail()
            {
                Email = email,
                Content = "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>"
            };

            if (newCode != null && userId != null)
            {
                await _sendEmailProxy.SendConfirmationEmail(newModel);
                return View("DisplayEmail");
            }
            return RedirectToLocal(null);
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterUserContract model)
        {
            if (ModelState.IsValid)
            {
                var response = await _proxy.Register(model);

                if (response.IsSuccessStatusCode)
                {
                    if (await SendConfirmationEmail(model.Email) != null)
                    {
                        ViewData["Email"] = model.Email;
                        return View("DisplayEmail");
                    }
                }
                else
                {
                    var ex = ApiExceptionValues.CreateApiException(response);
                    var detailedError = ex.Errors;
                    foreach (var error in detailedError)
                    {
                        AddErrors(IdentityResult.Failed(error));
                    }
                    return View();
                }
                //var user = new ApplicationUser() { UserName = model.UserName };
                //var result = await UserManager.CreateAsync(user, model.Password);
            }

            //if (ModelState.IsValid)
            //{
            //    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            //    var result = await UserManager.CreateAsync(user, model.Password);
            //    if (result.Succeeded)
            //    {
            //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

            //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            //        // Send an email with this link
            //        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            //        return RedirectToAction("Index", "Home");
            //    }
            //    AddErrors(result);
            //}

            // If we got this far, something failed, redisplay form

            return View(model);
        }


        public ActionResult Manage()
        {
            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            userId = Regex.Replace(userId, "\"", "");
            code = Regex.Replace(code, "\"", "");

            var newModel = new AccountBindingContract.ConfirmationContractModel()
            {
                Code = code,
                UserId = userId
            };

            var response = await _proxy.ConfirmEmail(newModel);
            return View(response.IsSuccessStatusCode ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(AccountBindingContract.ForgotPasswordContractModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _proxy.GetForgotPasswordCode(model);

            if (response != null)
            {
                var newCode = response;
                var userId = await _proxy.GetUserIdByEmail(model.Email);

                var callbackUrl = Url.Action("ResetPassword", "Account", new { UserId = userId, code = newCode },
                    protocol: Request.Url.Scheme);

                var newModel = new AccountBindingContract.SendEmail()
                {
                    Email = model.Email,
                    Content = "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>"
                };

                if (userId != null)
                {
                    await _sendEmailProxy.SendForgottenPasswordEmail(newModel);
                    return View("ForgotPasswordConfirmation", null, model.Email);
                }
            }

            //var user = await UserManager.FindByNameAsync(model.Email);
            //    if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            //    {
            //        // Don't reveal that the user does not exist or is not confirmed
            //        return View("ForgotPasswordConfirmation");
            //    }

            //    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            //    // Send an email with this link
            //    // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            //    // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
            //    // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
            //    // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            //}
            ModelState.AddModelError(string.Empty, "Could not find the email!");
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code, string userId)
        {
            return userId == null && code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(AccountBindingContract.ResetPasswordContractModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Code = Regex.Replace(model.Code, "\"", "");
            model.UserId = Regex.Replace(model.UserId, "\"", "");
            var response = await _proxy.ResetPassword(model);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        //
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        //
        // GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // If the user does not have an account, then prompt the user to create an account
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}


        //Managing Account Controllers
        #region

        //For Returning EditProfilePartialView
        public async Task<PartialViewResult> ShowMyProfile()
        {
            var token = GetToken();
            var userContract = await _proxy.GetUserData(token);
            return PartialView("Partial/EditProfilePartial", userContract);
        }

        public async Task<string> GetUserImage()
        {
            var token = GetToken();
            var userContract = await _proxy.GetUserData(token);
            return userContract.Image;
        }

        public async Task<ActionResult> GetUserData()
        {
            var token = GetToken();
            var userContract = await _proxy.GetUserData(token);
            return Json(userContract, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> IsFollowing(string userName)
        {
            var token = GetToken();
            var userContract = await _proxy.IsFollowing(userName, token);
            return Json(userContract, JsonRequestBehavior.AllowGet);

        }

        public async Task<ActionResult> Follow(string userName)
        {
            var token = GetToken();
            var response = await _proxy.Follow(userName, token);
            if (response.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public async Task<ActionResult> UnFollow(string userName)
        {
            var token = GetToken();
            var response = await _proxy.Unfollow(userName, token);
            if (response.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //POST: sending data to server
        [HttpPost]
        public async Task<ActionResult> EditMyProfile(UserContract model)
        {
            try
            {
                var token = GetToken();
                var response = await _proxy.Manage(model, token);
                return new HttpStatusCodeResult(response.StatusCode);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostProfileImage(string userName)
        {
            try
            {
                bool success = true;
                if (Request.Files.Count > 0)
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase image = files[0];
                    if (image.ContentLength > 0)
                    {
                        if (IsImage(image))
                        {
                            var resizedImage = ResizeImageToSmall(image, userName);

                            var result = UploadFile(resizedImage);
                            if (result.StatusCode == 200)
                            {
                                var patchUserImage = await _proxy.PatchAccountImage("Small_" + userName + ".jpg", userName, GetToken());
                                if (patchUserImage.StatusCode == HttpStatusCode.OK)
                                {
                                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                                }
                            }
                        }
                    }
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveProfileImage()
        {
            try
            {
                var token = GetToken();
                var response = await _proxy.RemoveProfileImage(token);
                if (response.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        [HttpGet]
        public async Task<ActionResult> GetFollowing(string userName)
        {
            try
            {
                var token = GetToken();
                var response = await _proxy.GetFollowing(userName, token);
                ViewData["follow"] = "Following";
                return PartialView("Partial/Account/FollowPartial", response);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetFollowers(string userName)
        {
            try
            {
                var token = GetToken();
                var response = await _proxy.GetFollowers(userName, token);
              
                ViewData["follow"] = "Follower";
                return PartialView("Partial/Account/FollowPartial", response);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public HttpPostedFileBase ResizeImageToSmall(HttpPostedFileBase file, string fileName)
        {
            ImageUtilities smallImage = new ImageUtilities();

            var filename = Path.GetFileName(file.FileName);
            var image = Image.FromStream(file.InputStream);
            double height = image.Height;
            double width = image.Width;

            if (height > width)
            {
                double ratioX = 75 / height;
                height = 75;
                width = width * ratioX;

            }
            else if (width > height)
            {
                double ratioY = 75 / width;
                width = 75;
                height = height * ratioY;
            }
            else
            {
                width = 75;
                height = 75;
            }

            int newWidth = (int)width;
            int newHeight = (int)height;

            var newImage = smallImage.ResizeImageWithQuality(image, newHeight, newWidth);

            image.Dispose();

            var newFileName = string.Format("{0}{1}{2}", "Small_", fileName, ".jpg");

            var response = ChangeToHttpFileBase(newImage, newFileName);

            return response;
        }

        private static HttpStatusCodeResult UploadFile(HttpPostedFileBase file)
        {
            var storageAccount = ConfigurationManager.AppSettings["BlobStorageAccountName"];

            var storageKey = ConfigurationManager.AppSettings["BlobStorageAccountKey"];

            var blobAcess = new AzureBlobAdapter(storageAccount, storageKey);

            var files = new List<HttpPostedFileBase> { file };

            var response = blobAcess.Upload(files, "hamroktmcontainer/UserImages");

            if (response.StatusCode == (int)HttpStatusCode.OK)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        private static MemoryFile ChangeToHttpFileBase(Bitmap newImage, string newFileName)
        {
            var format = ImageFormat.Jpeg;

            var mime = string.Format("Image/{0}", format);

            var memoryStream = new MemoryStream();

            newImage.Save(memoryStream, format);


            MemoryFile newFile = new MemoryFile(memoryStream, mime, newFileName);

            return newFile;
        }


        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" }; // add more if u like...


            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        // GET: /Manage/ChangePassword
        public ActionResult GetChangePasswordPartial()
        {
            return PartialView("Partial/ChangePasswordPartial");
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        public async Task<ActionResult> ChangePassword(AccountBindingContract.ChangePasswordBindingContractModel model)
        {
            if (!ModelState.IsValid)
            {

                var message = string.Join("<br>", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
            }
            try
            {
                var token = GetToken();
                var response = await _proxy.ChangePassword(model, token);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new HttpStatusCodeResult(response.StatusCode);
                }

                var ex = ApiExceptionValues.CreateApiException(response);
                var detailedError = ex.Errors;
                var message = "";
                foreach (var error in detailedError)
                {
                    message = string.Join("<br>", error);
                }
                return new HttpStatusCodeResult(response.StatusCode, message);

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        #endregion

        // POST: /Account/LogOff

        public async Task<ActionResult> LogOff()
        {
            await LogOffTask();

            return RedirectToAction("Index", "Home");
        }

        public async Task LogOffTask()
        {
            var token = GetToken();
            var response = await _proxy.LogOut(token);

            if (token != null)
            {
                HttpCookie tokenCookie = System.Web.HttpContext.Current.Request.Cookies["ebsCookie"];
                System.Web.HttpContext.Current.Response.Cookies.Remove("ebsCookie");
                tokenCookie.Expires = DateTime.Now.AddDays(-10);
                tokenCookie.Value = null;
                System.Web.HttpContext.Current.Response.SetCookie(tokenCookie);
            }

            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_userManager != null)
        //        {
        //            _userManager.Dispose();
        //            _userManager = null;
        //        }

        //        if (_signInManager != null)
        //        {
        //            _signInManager.Dispose();
        //            _signInManager = null;
        //        }
        //    }

        //    base.Dispose(disposing);
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            //public override void ExecuteResult(ControllerContext context)
            //{
            //    var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            //    if (UserId != null)
            //    {
            //        properties.Dictionary[XsrfKey] = UserId;
            //    }
            //    context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            //}
        }
        #endregion
    }
}