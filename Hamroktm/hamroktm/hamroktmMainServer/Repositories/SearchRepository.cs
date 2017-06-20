using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using contracts.Models;
using hamroktmMainServer.Database.Public;
using Db = hamroktmMainServer.Database.Public;

namespace hamroktmMainServer.Repositories
{
    public interface ISearchRepository
    {
        List<AdContract> SimpleSearch(List<string> searchTags);
        AdContract MaptoAdContract(Ad model);
    }

    public class SearchRepository : BaseRepository,ISearchRepository
    {
        private DbSet<Db.SearchTag> searchTagsDbSet { get; set; }

        public SearchRepository()
        {
            searchTagsDbSet = hamroktmDb.SearchTags;
        }

        //matches one word per ad and gets all result..like if user searches for "iphone cover". This will give results for iphone separately and cover separately.
        //public List<AdContract> SimpleSearch(List<string> searchTags)
        //{
        //    var searchedAds = new List<AdContract>();
        //    foreach (var tag in searchTags)
        //    {
        //        var test = searchTagsDbSet.Where(x => x.Ad.EndOn>DateTime.Now &&x.Name.Contains(tag));
        //        var resultsForTag = test.GroupBy(i => i.AdId).Select(g => g.FirstOrDefault()).ToList();
        //        if (resultsForTag.Any())
        //        {
        //            foreach (var ad in resultsForTag)
        //            {
        //                    var resultsforAd = MaptoAdContract(ad.Ad);
        //                    searchedAds.Add(resultsforAd);
        //            }
        //        }
        //    }
        //    var refinedSearch = searchedAds.GroupBy(x => x.AdId).Select(g => g.FirstOrDefault()).ToList();

        //    return refinedSearch.Any()
        //        ? refinedSearch
        //        : null;
        //}


        //refines the search with first keyword, gets the result. if anymore keyword, refines again.. so the result contains every word the user searched for
        public List<AdContract> SimpleSearch(List<string> searchTags)
        {
            var searchedAds = new List<AdContract>();
            var allliveAdsWithTag = searchTagsDbSet.Where(x => x.Ad.EndOn > DateTime.Now && x.Ad.Status=="Live").ToList();

            if (searchTags.Any())
            {
                //checks if the ad contains every keyword that user searched for
                foreach (var tag in searchTags)
                {
                    if (allliveAdsWithTag.Any()&&tag!=null)
                    {
                        //selects ad Id where tag matches
                        var tempAdId = allliveAdsWithTag.Where(x => x.Name.Contains(tag)).Select(x => x.AdId);

                        var adswithTag = new List<SearchTag>();

                        //loops through the ad Id and if it matches, adds to the list 
                        foreach (var id in tempAdId)
                        {
                            adswithTag.AddRange(allliveAdsWithTag.Where(x => x.AdId == id));
                        }

                        //replaces the source with new data for next search refining
                        allliveAdsWithTag = adswithTag;
                    }
                }
            }

            var resultsForTag = new List<SearchTag>();

            //refining if there are multiple record of same ad id.
            if (allliveAdsWithTag.Any())
            {
                resultsForTag = allliveAdsWithTag.GroupBy(i => i.AdId).Select(g => g.FirstOrDefault()).ToList();
            }

            if (resultsForTag.Any())
            {
                foreach (var ad in resultsForTag)
                {
                    var resultsforAd = MaptoAdContract(ad.Ad);
                    searchedAds.Add(resultsforAd);
                }
            }

            var refinedSearch = searchedAds.GroupBy(x => x.AdId).Select(g => g.FirstOrDefault()).ToList();

            return refinedSearch.Any()
                ? refinedSearch
                : null;
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

            if (model.AdTags != null)
            {
                var separatedTags = model.AdTags.Split(',');

                foreach (var separateTag in separatedTags)
                {
                    newAdContract.Tags.Add(separateTag);

                }

            }
            return newAdContract;
        }

    }
}