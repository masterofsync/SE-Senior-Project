using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
    public interface IAdRepository
    {
        HttpStatusCodeResult PostAd(AdContract model, List<string> searchTags, string userSubmittedTags);
        HttpStatusCodeResult EditAd(AdContract model, List<string> searchTags, string userSubmittedTags);
        HttpStatusCodeResult DeleteAd(int id, string userName, bool isAdmin);
        AdContract GetAdById(int id);
        List<AdContract> GetMyAds(string userName);
        List<AdContract> GetAllAdByUser(string userName);
        List<AdContract> GetPopularAds();
        List<AdContract> GetRecentAds();
        List<AdContract> GetFeaturedAds();
        HttpStatusCodeResult UpdateViews(int id);
        HttpStatusCodeResult MakeAdFeatured(int id, string currentUserName, bool isAdmin);
        HttpStatusCodeResult RemoveAdFromFeatured(int id, string currentUserName, bool isAdmin);
        HttpStatusCodeResult PauseAd(int id, string currentUserName, bool isAdmin);
        HttpStatusCodeResult UnpauseAd(int id, string curentUserName, bool isAdmin);
        HttpStatusCodeResult RenewAd(int id, DateTime endOn, string currentUserName, bool isAdmin);
        List<AdContract> GetAdsBySubCategory(string category, string subCategory);
        string getAdCreator(int adId);
        AdContract MaptoAdContract(Ad model);
        HttpStatusCodeResult UpdateToAdExpiredStatus();
        HttpStatusCodeResult UpdateToAdLiveStatus();
    }

    public class AdRepository : BaseRepository, IAdRepository
    {
        private DbSet<Db.Ad> AdsDbSet { get; set; }
        private DbSet<Db.Ad_Images> Ad_ImagesDbSet { get; set; }
        private DbSet<Db.SearchTag> SearchTagDbsSet { get; set; }

        public AdRepository()
        {
            AdsDbSet = hamroktmDb.Ads;
            Ad_ImagesDbSet = hamroktmDb.Ad_Images;
            SearchTagDbsSet = hamroktmDb.SearchTags;
        }

        //Creating or Posting Ad given model
        public HttpStatusCodeResult PostAd(AdContract model, List<string> searchTags, string userSubmittedTags)
        {
                using (var db = hamroktmDb)
                {
                    var adver = new Ad()
                    {
                        Price = model.Price,
                        Description = model.Description,
                        Category = model.Category,
                        Condition = model.Condition,
                        Title = model.Title,
                        CreatedBy = model.CreatedBy,
                        CreatedOn = model.CreatedOn,
                        Status = model.Status,
                        SubCategory = model.SubCategory,
                        Views = model.Views,
                        UpdatedOn = model.CreatedOn,
                        UpdatedBy = model.CreatedBy,
                        Featured = model.Featured,
                        AdTags = userSubmittedTags,
                        EndOn = model.EndOn,
                        StartOn = model.StartOn

                    };

                    AdsDbSet.Add(adver);
                    int i = 0;
                    foreach (var image in model.Images)
                    {

                        Ad_ImagesDbSet.Add(new Ad_Images()
                        {
                            AdId = adver.AdId,
                            ImageName = image,
                            Position = i

                        });
                        i++;
                    }

                    foreach (var tag in searchTags)
                    {
                        SearchTagDbsSet.Add(new SearchTag()
                        {
                            AdId = adver.AdId,
                            Name = tag
                        });
                    }

                    return SaveChanges();
                }
        }

        public HttpStatusCodeResult EditAd(AdContract model, List<string> searchTags, string userSubmittedTags)
        {
                Db.Ad dbPost;

                dbPost = AdsDbSet.FirstOrDefault(x => x.AdId == model.AdId);

                if (dbPost == default(Db.Ad))
                {
                    return null;
                }

                var existingAdImages = Ad_ImagesDbSet.Where(x => x.AdId == model.AdId);

                foreach (var existingImage in existingAdImages)
                {
                    Ad_ImagesDbSet.Remove(existingImage);
                }

                var existingSearchTags = SearchTagDbsSet.Where(x => x.AdId == model.AdId);

                foreach (var existingTag in existingSearchTags)
                {
                    SearchTagDbsSet.Remove(existingTag);
                }

                SaveChanges();

                dbPost.AdId = model.AdId;
                dbPost.Price = model.Price;
                dbPost.Description = model.Description;
                dbPost.Category = model.Category;
                dbPost.Condition = model.Condition;
                dbPost.Title = model.Title;
                dbPost.CreatedBy = model.CreatedBy;
                dbPost.CreatedOn = model.CreatedOn;
                dbPost.Status = model.Status;
                dbPost.SubCategory = model.SubCategory;
                dbPost.Views = model.Views;
                dbPost.UpdatedOn = model.CreatedOn;
                dbPost.UpdatedBy = model.CreatedBy;
                dbPost.Featured = model.Featured;
                dbPost.EndOn = model.EndOn;
                dbPost.StartOn = model.StartOn;
                dbPost.AdTags = userSubmittedTags;

                foreach (var newImage in model.Images)
                {
                    Ad_ImagesDbSet.Add(new Ad_Images()
                    {
                        AdId = dbPost.AdId,
                        ImageName = newImage,
                        Ad = dbPost
                    });
                }

                foreach (var newTag in searchTags)
                {
                    SearchTagDbsSet.Add(new SearchTag()
                    {
                        AdId = dbPost.AdId,
                        Name = newTag,
                        Ad = dbPost
                    });
                }
                return SaveChanges();
        }

        //Deleting Ad by AdId
        public HttpStatusCodeResult DeleteAd(int id, string userName, bool isAdmin)
        {
            var deleteAd = AdsDbSet.FirstOrDefault(x => x.AdId == id);

            if (deleteAd != null)
            {
                if (userName.Equals(deleteAd.CreatedBy, StringComparison.InvariantCultureIgnoreCase) || isAdmin)
                {
                    AdsDbSet.Remove(deleteAd);

                    //Get and Remove tag strings
                    var removeTagConnection = SearchTagDbsSet.Where(x => x.AdId == id);
                    if (removeTagConnection.Any())
                    {
                        SearchTagDbsSet.RemoveRange(removeTagConnection);
                    }
                    //Get and Remove image strings
                    var removeImageConnection = Ad_ImagesDbSet.Where(x => x.AdId == id);
                    if (removeImageConnection.Any())
                    {
                        Ad_ImagesDbSet.RemoveRange(removeImageConnection);
                    }
                    //removing all comments related to the ad.
                    var commentRepo = new CommentRepository();
                    commentRepo.DeleteAllAdComment(id);
                    return SaveChanges();
                }
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        //Getting Single Ad by AdId
        public AdContract GetAdById(int id)
        {
            var singleAd = AdsDbSet.FirstOrDefault(x => x.AdId == id);

            if (singleAd != null)
            {
                var adModel = MaptoAdContract(singleAd);
                return adModel;
            }
            return null;
        }

        //Getting all Users Ads
        public List<AdContract> GetMyAds(string userName)
        {
            var myAds = AdsDbSet.Where(x => x.CreatedBy == userName);
            var myAdsList = new List<AdContract>();
            foreach (var myAd in myAds)
            {
                var listItem = MaptoAdContract(myAd);
                myAdsList.Add(listItem);
            }

            if (myAdsList.Any())
            {
                return myAdsList;
            }
            return null;
        }     
        
        //Getting all Users Ads
        public List<AdContract> GetAllAdByUser(string userName)
        {
            var allAds = AdsDbSet.Where(x => x.CreatedBy == userName&&x.Status=="Live");
            var allAdsList = new List<AdContract>();
            foreach (var myAd in allAds)
            {
                var listItem = MaptoAdContract(myAd);
                allAdsList.Add(listItem);
            }

            if (allAdsList.Any())
            {
                return allAdsList;
            }
            return null;
        }

        public List<AdContract> GetPopularAds()
        {
            var popularAds = AdsDbSet.Where(x => x.Status == "Live").OrderByDescending(x => x.Views);
            var popularAdsList = new List<AdContract>();
            foreach (var popularAd in popularAds)
            {
                var listItem = MaptoAdContract(popularAd);
                popularAdsList.Add(listItem);
            }
            if (popularAdsList.Any())
            {
                return popularAdsList;
            }
            return null;
        }

        //public JsonResult GetRecentAds(int pageSize = 5, int currentPage = 1)
        public List<AdContract> GetRecentAds()
        {
            var recentAds = AdsDbSet.Where(x => x.Status == "Live").OrderByDescending(x => x.CreatedOn);

            var recentAdsList = new List<AdContract>();

            foreach (var recentAd in recentAds)
            {
                var listItem = MaptoAdContract(recentAd);
                recentAdsList.Add(listItem);
            }

            if (recentAdsList.Any())
            {
                return recentAdsList;
            }
            return null;
        }

        public List<AdContract> GetFeaturedAds()
        {
            var featuredAds = AdsDbSet.Where(x => x.Status == "Live" && x.Featured == true);
            var featuredAdsList = new List<AdContract>();
            foreach (var popularAd in featuredAds)
            {
                var listItem = MaptoAdContract(popularAd);
                featuredAdsList.Add(listItem);
            }
            if (featuredAdsList.Any())
            {
                return featuredAdsList;
            }
            return null;
        }

        public HttpStatusCodeResult UpdateViews(int id)
        {
            var toPatch = AdsDbSet.SingleOrDefault(x => x.AdId == id);
            toPatch.Views += 1;
            return SaveChanges();
        }

        public HttpStatusCodeResult MakeAdFeatured(int id, string currentUserName,bool isAdmin)
        {
            var toPatch = AdsDbSet.SingleOrDefault(x => x.AdId == id);
            if (toPatch != null)
            {
                if (toPatch.Featured == true)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }
                if (currentUserName.Equals(toPatch.CreatedBy, StringComparison.InvariantCultureIgnoreCase) || isAdmin)
                {
                    toPatch.Featured = true;
                    return SaveChanges();
                }
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return null;
        }

        public HttpStatusCodeResult RemoveAdFromFeatured(int id, string currentUserName, bool isAdmin)
        {
            var toPatch = AdsDbSet.SingleOrDefault(x => x.AdId == id);
            if (toPatch != null)
            {
                if (currentUserName.Equals(toPatch.CreatedBy, StringComparison.InvariantCultureIgnoreCase)|| isAdmin)
                {
                    if (toPatch.Featured == true)
                    {
                        toPatch.Featured = false;
                        return SaveChanges();
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        public HttpStatusCodeResult PauseAd(int id,string currentUserName,bool isAdmin )
        {
            var toPatch = AdsDbSet.SingleOrDefault(x => x.AdId == id);
            if (toPatch != null)
            {
                if (toPatch.Status == "Paused")
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }
                if (currentUserName.Equals(toPatch.CreatedBy, StringComparison.InvariantCultureIgnoreCase) || isAdmin)
                {
                    toPatch.Status = "Paused";
                    return SaveChanges();
                }
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        public HttpStatusCodeResult UnpauseAd(int id,string currentUserName,bool isAdmin)
        {
            var toPatch = AdsDbSet.SingleOrDefault(x => x.AdId == id);
            if (toPatch != null)
            {
                if (currentUserName.Equals(toPatch.CreatedBy, StringComparison.InvariantCultureIgnoreCase) || isAdmin)
                {
                    if (toPatch.Status == "Paused")
                    {
                        if (toPatch.EndOn >= DateTime.Now)
                        {
                            toPatch.Status = "Live";
                        }
                        else
                        {
                            toPatch.Status = "Expired";
                        }
                        return SaveChanges();
                    }
                }
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        public HttpStatusCodeResult RenewAd(int id, DateTime endOn,string currentUserName,bool isAdmin)
        {
            var adtoUpdate = AdsDbSet.SingleOrDefault(x => x.AdId == id);
            if (adtoUpdate != null)
            {

                if (currentUserName.Equals(adtoUpdate.CreatedBy, StringComparison.InvariantCultureIgnoreCase) || isAdmin)
                {
                    adtoUpdate.EndOn = endOn;
                    adtoUpdate.Status = "Live";
                    adtoUpdate.UpdatedOn = DateTime.Now;
                    return SaveChanges();
                }

                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        //Refactor this when Ad table has FK of Category and SubCategory
        public List<AdContract> GetAdsBySubCategory(string category, string subCategory)
        {
            if (category != null && subCategory != null)
            {
                var adsBySubCategory = AdsDbSet.Where(x => x.Category == category && x.SubCategory == subCategory && x.Status == "Live" );

                if (adsBySubCategory.Any())
                {
                    var finalList = new List<AdContract>();

                    foreach (var ads in adsBySubCategory)
                    {
                        finalList.Add(MaptoAdContract(ads));
                    }

                    return finalList;
                }
                return null;
            }
            return null;
        }

        public string getAdCreator(int adId)
        {
            var ad = AdsDbSet.FirstOrDefault(x => x.AdId == adId);
            if (ad != null)
            {
                return ad.CreatedBy;
            }
            return null;
        }

        public AdContract MaptoAdContract(Ad model)
        {
            var newAdContract = new AdContract()
            {
                AdId = model.AdId,
                Price = model.Price,
                Description = model.Description,
                Category = model.Category,
                Condition = model.Condition,
                Title = model.Title,
                CreatedBy = model.CreatedBy,
                CreatedOn = model.CreatedOn,
                Status = model.Status,
                SubCategory = model.SubCategory,
                Views = model.Views,
                UpdatedOn = model.UpdatedOn,
                UpdatedBy = model.UpdatedBy,
                EndOn = model.EndOn,
                StartOn = model.StartOn,
                Featured = model.Featured

            };

            if (model.Ad_Images.Any())
            {
                foreach (var images in model.Ad_Images)
                {
                    newAdContract.Images.Add(images.ImageName);
                }
            }

            if (model.AdTags != "")
            {
                var separatedTags = model.AdTags.Split(',');

                foreach (var separateTag in separatedTags)
                {
                    newAdContract.Tags.Add(separateTag);

                }

            }
            return newAdContract;
        }

        public HttpStatusCodeResult UpdateToAdExpiredStatus()
        {
            var allAds = AdsDbSet.Where(x => x.Status == "Live" && x.EndOn < DateTime.Now);
            foreach (var ad in allAds)
            {
                ad.Status = "Expired";
            }

            return SaveChanges();
        }

        public HttpStatusCodeResult UpdateToAdLiveStatus()
        {
            var allAds = AdsDbSet.Where(x => x.Status == "Onhold" && x.StartOn <= DateTime.Now );
            foreach (var ad in allAds)
            {
                ad.Status = "Live";
            }
            return SaveChanges();
        }
    }
}