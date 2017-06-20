using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using contracts.Models;
using hamroktmMainServer.Models;
using hamroktmMainServer.Repositories;
using Microsoft.AspNet.Identity;

namespace hamroktmMainServer.Controllers
{
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/Comment")]
    public class CommentController : BaseController
    {
        private readonly ICommentRepository _repo;
        private readonly AdController _adController;

        public CommentController(ICommentRepository repo, AdController adController)
        {
            _repo = repo;
            _adController = adController;
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("PostComment")]
        public IHttpActionResult PostComment(AdCommentContract model)
        {
            if (ModelState.IsValid)
            {
                var userName = RequestContext.Principal.Identity.GetUserName();
                var userId = RequestContext.Principal.Identity.GetUserId();
                if (userName.Equals(model.CreatedBy, StringComparison.InvariantCultureIgnoreCase))
                {
                    var response = _repo.PostComment(model, userId);

                    if (response.StatusCode == 200)
                    {
                        var adCreator = _adController.GetAdCreator(model.AdId);
                        ApplicationUser user = UserManager.FindByName(adCreator);
                        var link = ConfigurationManager.AppSettings["currentEnvironmentContext"] + "ad/" + model.AdId;
                        var emailContent = new AccountBindingContract.SendEmail()
                        {
                            Email = user.Email,
                            Content = userName + " posted a comment in your <a href='" + link + "'>Ad</a>",
                            Subject = "EasilyBuyAndSell: Comment posted on your ad!"
                        };
                        SendEmail(emailContent);
                        return Ok();
                    }
                    return BadRequest();
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("GetAllAdComments")]
        public HttpResponseMessage GetAllAdComments(int adId)
        {
            var response = _repo.GetallAdComments(adId);
            if (response != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("DeleteComment")]
        public IHttpActionResult DeleteComment(int commentId)
        {
            var userId = RequestContext.Principal.Identity.GetUserId();
            var userName = RequestContext.Principal.Identity.GetUserName();
            var isAdmin = RequestContext.Principal.IsInRole("Admin");
            var response = _repo.DeleteComment(commentId, userId, userName,isAdmin);

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

    }
}