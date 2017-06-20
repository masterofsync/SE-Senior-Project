using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using contracts.Models;
using hamroktmMainServer.Database.Public;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Db = hamroktmMainServer.Database.Public;


namespace hamroktmMainServer.Repositories
{
    public interface IAccountRepository
    {
        UserContract GetAdPoster(string userName);
        HttpStatusCodeResult PatchAccountImage(string imageName, string userName);
        bool IsFollowing(string followerId, string followingId);
        List<FollowContract> GetAllFollowers(string userId);
        List<FollowContract> GetAllFollowings(string userId);
        int GetFollowersCount(string userId);
        int GetFollowingCount(string userId);
        HttpStatusCodeResult Follow(string followerId, string followingId);
        HttpStatusCodeResult UnFollow(string followerId, string followingId);
        UserContract MapToUserContract(AspNetUser dbModel);


    }

    public class AccountRepository : BaseRepository,IAccountRepository
    {
        private DbSet<Db.AdPoster> AdPoster { get; set; }
        private DbSet<Db.AspNetUser> AccountUser { get; set; }
        private DbSet<Db.Users_Follow> UserFollow { get; set; }

        public AccountRepository()
        {
            AdPoster = hamroktmDb.AdPosters;
            AccountUser = hamroktmDb.AspNetUsers;
            UserFollow = hamroktmDb.Users_Follow;

        }

        public UserContract GetAdPoster(string userName)
        {
            var poster = AdPoster.FirstOrDefault(x => x.UserName == userName);
            if (poster != null)
            {
                return new UserContract()
                {
                    Email = poster.Email,
                    FirstName = poster.FirstName,
                    UserName = poster.UserName,
                    LastName = poster.LastName,
                    Phone = poster.PhoneNumber,
                    CreatedOn = poster.CreatedOn,
                    Image = poster.Image
                };
            }
            return null;
        }

        public HttpStatusCodeResult PatchAccountImage(string imageName, string userName)
        {

            var user = AccountUser.FirstOrDefault(x => x.UserName == userName);
            if (user != null)
            {
                user.Image = imageName;
                user.UpdatedOn = DateTime.Now;
                return SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public bool IsFollowing(string followerId, string followingId)
        {
            var checkFollow = UserFollow.FirstOrDefault(x => x.FollowerId == followerId && x.FollowingId == followingId);
            if (checkFollow != null)
            {
                return true;
            }
            return false;
        }


        public List<FollowContract> GetAllFollowers(string userId)
        {
            var followers = UserFollow.Where(x => x.FollowingId == userId);
            var followersList = new List<FollowContract>();

            if (followers.Any())
            {
                foreach (var follower in followers)
                {
                    var user = AccountUser.FirstOrDefault(x => x.Id == follower.FollowerId);
                    if (user != null)
                    {
                        var newModel = new FollowContract()
                        {
                            IsFollowing = IsFollowing(userId,user.Id),
                            UserData = MapToUserContract(user)
                        };
                        followersList.Add(newModel);
                    }
                }
            }
            return followersList;
        }

        public List<FollowContract> GetAllFollowings(string userId)
        {
            var followings = UserFollow.Where(x => x.FollowerId == userId);
            var followingList = new List<FollowContract>();

            if (followings.Any())
            {
                foreach (var following in followings)
                {
                    var user = AccountUser.FirstOrDefault(x => x.Id == following.FollowingId);
                    if (user != null)
                    {
                        var newModel = new FollowContract()
                        {
                            //it should be always true since we already got found the user
                            IsFollowing = true,
                            UserData = MapToUserContract(user)
                        };
                        followingList.Add(newModel);
                    }
                }
            }
            return followingList;
        }

        public int GetFollowersCount(string userId)
        {
            int followerCount = UserFollow.Count(x => x.FollowingId == userId);
            return followerCount;
        }

        public int GetFollowingCount(string userId)
        {
            int followingCount = UserFollow.Count(x => x.FollowerId == userId);
            return followingCount;
        }

        public HttpStatusCodeResult Follow(string followerId, string followingId)
        {
            var alreadyExists = UserFollow.FirstOrDefault(x => x.FollowerId == followerId && x.FollowingId == followingId);

            if (alreadyExists==null)
            {
                var userFollow = new Users_Follow()
                {
                    FollowerId = followerId,
                    FollowingId = followingId,
                    CreatedOn = DateTime.Now
                    
                };
                UserFollow.Add(userFollow);
                return SaveChanges();
            }
            return null;
        } 
        
        public HttpStatusCodeResult UnFollow(string followerId, string followingId)
        {
            var exists = UserFollow.FirstOrDefault(x => x.FollowerId == followerId && x.FollowingId == followingId);

            if (exists != null)
            {
                UserFollow.Remove(exists);
                return SaveChanges();
            }

            return null;
        }

        public UserContract MapToUserContract(AspNetUser dbModel)
        {
            var mappedContract = new UserContract()
            {
                Email = dbModel.Email,
                UserName = dbModel.UserName,
                FirstName = dbModel.FirstName,
                Id = dbModel.Id,
                Image = dbModel.Image,
                Phone = dbModel.PhoneNumber,
                LastName = dbModel.LastName,
                CreatedOn = dbModel.CreatedOn
            };

            foreach (var role in dbModel.AspNetRoles)
            {
                mappedContract.Role.Add(role.Name);
            }

            return mappedContract;
        }

    }
}