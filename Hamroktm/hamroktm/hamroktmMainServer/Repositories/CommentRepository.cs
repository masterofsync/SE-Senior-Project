using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Db = hamroktmMainServer.Database.Public;
using contracts.Models;
using Microsoft.AspNet.Identity;

namespace hamroktmMainServer.Repositories
{
    public interface ICommentRepository
    {
        HttpStatusCodeResult PostComment(AdCommentContract comment, string userId);
        List<AdCommentContract> GetallAdComments(int adId);
        HttpStatusCodeResult DeleteAllAdComment(int adId);
        HttpStatusCodeResult DeleteComment(int commentId, string userId, string currentUserName, bool isAdmin);
        AdCommentContract MaptoAdComment(Db.AdComment model);
    }

    public class CommentRepository : BaseRepository, ICommentRepository
    {
        private DbSet<Db.AdComment> CommentDbSet { get; set; }

        public CommentRepository()
        {
            CommentDbSet = hamroktmDb.AdComments;
        }

        //post a comment
        public HttpStatusCodeResult PostComment(AdCommentContract comment, string userId)
        {
            using (var db = hamroktmDb)
            {
                var commentToPost = new Db.AdComment()
                {
                    AdId = comment.AdId,
                    Description = comment.Description,
                    CreatedById = userId,
                    CreatedOn = DateTime.Now
                };
                CommentDbSet.Add(commentToPost);
                return SaveChanges();
            }
        }

        //Get all comments for an ad
        public List<AdCommentContract> GetallAdComments(int adId)
        {
            var allCommentsforAd = CommentDbSet.Where(x => x.AdId == adId);
            var allCommentsList = new List<AdCommentContract>();

            foreach (var comment in allCommentsforAd)
            {
                var mappedComment = MaptoAdComment(comment);
                allCommentsList.Add(mappedComment);
            }

            if (allCommentsList.Any())
            {
                return allCommentsList;
            }
            return null;

        }

        //Delete all comment for an ad
        public HttpStatusCodeResult DeleteAllAdComment(int adId)
        {
            var allCommentsforAd = CommentDbSet.Where(x => x.AdId == adId);

            if (allCommentsforAd.Any())
            {
                foreach (var comment in allCommentsforAd)
                {
                    CommentDbSet.Remove(comment);
                }
                return SaveChanges();
            }

            return null;
        }


        //Delete a comment by user or admin.
        public HttpStatusCodeResult DeleteComment(int commentId,string userId,string currentUserName,bool isAdmin)
        {
            var deleteComment = CommentDbSet.FirstOrDefault(x => x.CommentId == commentId);
            var adRepo = new AdRepository();
            if (deleteComment != null)
            {
                //if the person wanting to delete the comment is the ad creator
                //find a way to not access the others repository directly
                bool isAdCreator = adRepo.getAdCreator(deleteComment.AdId) == currentUserName;
                if (userId == deleteComment.CreatedById || isAdmin || isAdCreator)
                {
                    CommentDbSet.Remove(deleteComment);
                    return SaveChanges();
                }
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        //mapping database data to contract data
        public AdCommentContract MaptoAdComment(Db.AdComment model)
        {
            //var user = UserManager.FindByIdAsync(model.CreatedById);
            //var userName = user.Result.UserName;
            var newComment = new AdCommentContract()
            {
                AdId = model.AdId,
                CreatedBy = model.AspNetUser.UserName,
                Description = model.Description,
                CreatedOn = model.CreatedOn,
                CommentId = model.CommentId,
                UserImage = model.AspNetUser.Image
            };

            return newComment;
        }
    }
}