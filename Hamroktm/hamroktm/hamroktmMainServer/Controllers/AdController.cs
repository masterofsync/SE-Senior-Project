using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Design;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using contracts.Models;
using hamroktmMainServer.Models;
using hamroktmMainServer.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using hamroktm.Helpers;

namespace hamroktmMainServer.Controllers
{

    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/Ad")]
    public class AdController : BaseController
    {
        private readonly IAdRepository _repo;
        private readonly AccountController _accountController;
        public AdController(IAdRepository repo, AccountController accountController)
        {
            _repo = repo;
            _accountController = accountController;
        }
       
        // POST:Ad/PostAd
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("PostAd")]
        public async Task<IHttpActionResult> PostAd(AdContract model)
        {
            var userName = RequestContext.Principal.Identity.GetUserName();
            var userId = RequestContext.Principal.Identity.GetUserId();
            var user = await UserManager.FindByNameAsync(userName);
            //ApplicationUser user =   UserManager.FindByEmailAsync(email);

            if (userName.Equals(model.CreatedBy, StringComparison.InvariantCultureIgnoreCase))
            {
                var searchTags = CreateSearchTags(model.Title, model.Tags);
                var userSubmittedTags = String.Join(",", model.Tags);
                var response = _repo.PostAd(model, searchTags, userSubmittedTags);
                if (response.StatusCode == 200)
                {

                    //send notification to the followers.
                    //get all the followers email.
                    var _accountRepo = new AccountRepository();
                    var followers = _accountRepo.GetAllFollowers(userId);

                    //if any follower, compose an email.
                    if (followers != null)
                    {
                        var link = ConfigurationManager.AppSettings["currentEnvironmentContext"] +
                                   "ad/UsersAdList?userName=" + userName;

                        //send the email to all the followers.
                        foreach (var follower in followers)
                        {
                            var emailContent = "<p>Hello " + follower.UserData.FirstName + ",</p><p>" + userName +
                                               " has posted an Ad!</p><p>To view the users latest ads,<a href='" + link +
                                               "'> Click here</a></p><p>Regards,</p><p>EasilyBuyAndSell Team</p>";
                            var email = new AccountBindingContract.SendEmail()
                            {
                                Content = emailContent,
                                Email = follower.UserData.Email,
                                Subject = "EBS - Ad Posted By " + userName
                            };
                            SendEmail(email);
                        }
                    }
                    return Ok();
                }
                return BadRequest();
            }
            return Unauthorized();
        }

        // POST:Ad/EditAd
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("EditAd")]
        public async Task<IHttpActionResult> EditAd(AdContract model)
        {
            var userName = RequestContext.Principal.Identity.GetUserName();
            if (userName.Equals(model.CreatedBy, StringComparison.InvariantCultureIgnoreCase))
            {
                var searchTags = CreateSearchTags(model.Title, model.Tags);
                var userSubmittedTags = String.Join(",", model.Tags);

                var response = _repo.EditAd(model, searchTags, userSubmittedTags);
                if (response.StatusCode == 200)
                {
                    return Ok();
                }
                return BadRequest();
            }
            return Unauthorized();
        }

        //Delete Ad with Ad id - Check who is trying to delete(Admin? , or the user?)

        // POST: Ad/DeleteAd
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("DeleteAd")]
        public IHttpActionResult DeleteAd(int id)
        {
            var userName = RequestContext.Principal.Identity.GetUserName();
            var isAdmin = RequestContext.Principal.IsInRole("Admin");
            var response = _repo.DeleteAd(id, userName, isAdmin);
            if (response.StatusCode == 200)
            {
                return Ok();
            }
            if (response == new HttpStatusCodeResult(HttpStatusCode.Unauthorized))
            {
                return Unauthorized();
            }
            return BadRequest();
        }


        //Get Ad with Ad id
        // POST: Ad/GetAdById
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetAdById")]
        public HttpResponseMessage GetAdById(int id)
        {
            var response = _repo.GetAdById(id);

            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("UpdateToAdExpiredStatus")]
        public HttpResponseMessage UpdateToAdExpiredStatus()
        {
            var response = _repo.UpdateToAdExpiredStatus();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("UpdateToAdLiveStatus")]
        public HttpResponseMessage UpdateToAdLiveStatus()
        {
            var response = _repo.UpdateToAdLiveStatus();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetMyAds")]
        public HttpResponseMessage GetMyAds()
        {
            var userName = RequestContext.Principal.Identity.GetUserName();
            var response = _repo.GetMyAds(userName);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetAllAdByUser")]
        public HttpResponseMessage GetAllAdByUser(string userName)
        {
            var response = _repo.GetAllAdByUser(userName);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        //Get popular Ads- May be First 10 popular ads with View in SMSS and this be in Base Repository?
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetPopularAds")]
        public HttpResponseMessage GetPopularAds()
        {
            var response = _repo.GetPopularAds();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        //Get Latest Ads- May be First 10 latest ads with View in SMSS and this be in Base Repository?
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetRecentAds")]
        public HttpResponseMessage GetRecentAds()
        {
            var response = _repo.GetRecentAds();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetFeaturedAds")]
        public HttpResponseMessage GetFeaturedAds()
        {
            var response = _repo.GetFeaturedAds();
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("UpdateViews")]
        public IHttpActionResult UpdateViews(int id)
        {
            var response = _repo.UpdateViews(id);
            if (response.StatusCode == 200)
            {
                return Ok();
            }
            return BadRequest();
        }

        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("RenewAd")]
        public IHttpActionResult RenewAd(int id, DateTime endOn)
        {
            var isAdmin = RequestContext.Principal.IsInRole("Admin");
            var currentUser = RequestContext.Principal.Identity.GetUserName();
            var response = _repo.RenewAd(id, endOn,currentUser, isAdmin);
            if (response.StatusCode == 200)
            {
                return Ok();
            }
            if (response == new HttpStatusCodeResult(HttpStatusCode.Unauthorized))
            {
                return Unauthorized();
            }
            return BadRequest();
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("MakeDeal")]
        public IHttpActionResult MakeDeal(DealContract model)
        {
            var accountRepo = new AccountRepository();
            var user = accountRepo.GetAdPoster(GetAdCreator(model.AdId));
            var ad = _repo.GetAdById(model.AdId);
            var subject = "EasilyBuyAndSell: Deal - " + ad.Title;
            var newTitle = ad.Title.Replace("\'", "&#39;");
            var descrip = "<p>Hello " + user.FirstName + ",</p> <p>A deal has been put forward by '" + model.Name + "'.</p>" + "<p><b>Deal Details</b></p><p><b>Amount offered</b> : $" + model.DealAmount + "<br/><b>Marked Price</b> : $" + ad.Price + "<br/><b>Message</b> :" + model.Message + " </p><p><a href='mailto:" + model.Email + "?subject=EasilyBuyAndSell: Response - Deal for " + newTitle + "'> Click here</a> to respond.</p><p>Regards,</p><p>EasilyBuyAndSell Team</p>";
            var emailContent = new AccountBindingContract.SendEmail
            {
                Email = user.Email,
                Subject = subject,
                Content = descrip
            };
            return SendEmail(emailContent);
        }

        //refactor this later to admin controller
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("MakeAdFeatured")]
        [System.Web.Http.Authorize(Roles = "Admin")]
        public IHttpActionResult MakeAdFeatured(int id)
        {
            var isAdmin = RequestContext.Principal.IsInRole("Admin");
            var currentUser = RequestContext.Principal.Identity.GetUserName();
            var response = _repo.MakeAdFeatured(id, currentUser, isAdmin);
            if (response.StatusCode == 200)
            {
                return Ok();
            }
            if (response == new HttpStatusCodeResult(HttpStatusCode.Unauthorized))
            {
                return Unauthorized();
            }
            if (response.StatusCode == 409)
            {
                return Conflict();
            }
            return BadRequest();
        }

        //refactor this later to admin controller
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("RemoveAdFromFeatured")]
        [System.Web.Http.Authorize(Roles = "Admin")]
        public IHttpActionResult RemoveAdFromFeatured(int id)
        {
            var isAdmin = RequestContext.Principal.IsInRole("Admin");
            var currentUser = RequestContext.Principal.Identity.GetUserName();
            var response = _repo.RemoveAdFromFeatured(id, currentUser, isAdmin);
            if (response.StatusCode == 200)
            {
                return Ok();
            }
            if (response == new HttpStatusCodeResult(HttpStatusCode.Unauthorized))
            {
                return Unauthorized();
            }

            if (response.StatusCode == 404)
            {
                return NotFound();
            }
            return BadRequest();
        }

        //refactor this later to admin controller
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("PauseAd")]
        public IHttpActionResult PauseAd(int id)
        {
            var isAdmin = RequestContext.Principal.IsInRole("Admin");
            var currentUser = RequestContext.Principal.Identity.GetUserName();
            var response = _repo.PauseAd(id, currentUser, isAdmin);
            if (response.StatusCode == 200)
            {
                return Ok();
            }
            if (response == new HttpStatusCodeResult(HttpStatusCode.Unauthorized))
            {
                return Unauthorized();
            }
            if (response.StatusCode == 409)
            {
                return Conflict();
            }
            if (response.StatusCode == 404)
            {
                return NotFound();
            }
            return BadRequest();
        }

        //refactor this later to admin controller
        [System.Web.Http.HttpPatch]
        [System.Web.Http.Route("UnpauseAd")]
        public IHttpActionResult UnpauseAd(int id)
        {
            var isAdmin = RequestContext.Principal.IsInRole("Admin");
            var currentUser = RequestContext.Principal.Identity.GetUserName();
            var response = _repo.UnpauseAd(id, currentUser, isAdmin);
            if (response.StatusCode == 200)
            {
                return Ok();
            }
            if (response == new HttpStatusCodeResult(HttpStatusCode.Unauthorized))
            {
                return Unauthorized();
            }

            if (response.StatusCode == 404)
            {
                return NotFound();
            }
            return BadRequest();
        }

        //Get Ad creator username
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetAdCreator")]
        public string GetAdCreator(int adId)
        {
            var creator = _repo.getAdCreator(adId);
            return creator;
        }

        //creates final tags list for searching - for SearchTags table
        public List<string> CreateSearchTags(string title, List<string> AdTags)
        {
            var tool = new RegexUtilities();

            var newSearchTags = tool.CreateTagsFromText(title);

            foreach (var tag in AdTags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    newSearchTags.Add(tag.ToLower());
                }
            }
            return newSearchTags;
        }

        //Get All Ads- With Index Range?

        //Get popular Ads with Range index?

        //Get latest Ads with Range index?

        //Edit Ad with Ad id




    }
}