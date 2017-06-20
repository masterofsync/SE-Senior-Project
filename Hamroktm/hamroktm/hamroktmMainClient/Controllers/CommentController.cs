using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using contracts.Models;
using hamroktmMainClient.Proxies;
using hamroktmMainClient.ViewModels;

namespace hamroktmMainClient.Controllers
{
    [TokenAuthorize]
    public class CommentController : BaseController
    {
        private readonly ICommentProxy _commentProxy;
        private readonly AdController _adController;

        public CommentController(ICommentProxy proxy, AdController controller)
        {
            _commentProxy = proxy;
            _adController = controller;
        }
 

        [HttpPost]
        public async Task<ActionResult> PostComment(string comment, int adId)
        {
            var token = GetToken();

            var newModel = new AdCommentContract()
            {
                Description = comment,
                AdId = adId,
                CreatedBy = GetUserName(),
                CreatedOn = DateTime.Now
            };

            var response = await _commentProxy.PostComment(newModel, token);
            if (response.IsSuccessStatusCode)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error Posting the Comment! Please Try again later!");
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetAllAdComments(int adId)
        {
            var response = await _commentProxy.GetAllAdComments(adId);
            if (response == null)
            {
                return new HttpStatusCodeResult(200, "No Comments Posted Yet!");
            }
            //get ad creator
            var adCreator = await _adController.GetAdCreator(adId);
            response = response.OrderByDescending(x => x.CreatedOn);
            var newCommentViewModel = new CommentsViewModel()
            {
                AdComments = response,
                adCreator = adCreator
            };
            return PartialView("Partial/Comment/AdCommentsPartial", newCommentViewModel);
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetPostCommentSection()
        {
            if (GetToken() != null)
            {
                return PartialView("Partial/Comment/PostCommentPartial");
            }
            return new HttpStatusCodeResult(400, "Please <b>Log In</b> to post comments!");
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            var token = GetToken();
            var response =await _commentProxy.DeleteComment(commentId, token);
            return new HttpStatusCodeResult(response.StatusCode, "CommentDeleted!");
        }
    }
}