using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hamroktmMainClient.Proxies.Models
{
    public static class ProxyUriList
    {
        //Account
        //Login
        public static readonly string LoginUri = "token";
        //Register
        public static readonly string RegisterUri = "api/Account/Register";
        //Logout
        public static readonly string LogOutUri = "api/Account/Logout";
        //ManageProfile
        public static readonly string Manage = "api/Account/ManageProfile";
        //ChangePassword
        public static readonly string ChangePassword = "api/Account/ChangePassword";
        //ResetPassword
        public static readonly string ResetPassword = "api/Account/ResetPassword";
        //GetForgotPasswordCode
        public static readonly string GetForgotPasswordCode = "api/Account/GetForgotPasswordCode";
        //GetConfirmationCode
        public static readonly string GetConfirmationCode = "api/Account/GetConfirmationCode";
        //GetUserIdByEmail
        public static readonly string GetUserIdByEmail = "api/Account/GetUserIdByEmail";
        //GetUserIdByEmail
        public static readonly string GetUserNameByEmail = "api/Account/GetUserNameByEmail";
        //GetEmailByUserName
        public static readonly string GetEmailByUserName = "api/Account/GetEmailByUserName";
        //SendEmail
        public static readonly string SendEmail = "api/Account/SendEmail";
        //ConfirmEmail
        public static readonly string ConfirmEmail = "api/Account/ConfirmEmail";
        //CheckEmailConfirmation
        public static readonly string CheckEmailConfirmation = "api/Account/CheckEmailConfirmation";
        //GetUserData
        public static readonly string GetUserData = "api/Account/GetUserData";
        //GetAdPoster
        public static readonly string GetAdPoster = "api/Account/GetAdPoster"; 
        //PatchAccountImage
        public static readonly string PatchAccountImage = "api/Account/PatchAccountImage";
        //DeleteProfileImage
        public static readonly string RemoveProfileImage = "api/Account/RemoveProfileImage";
        //IsFollowing
        public static readonly string IsFollowing = "api/Account/IsFollowing";
        //Follow
        public static readonly string Follow = "api/Account/Follow";
        //UnFollow
        public static readonly string UnFollow = "api/Account/UnFollow";
        //GetAllFollowings
        public static readonly string GetAllFollowings = "api/Account/GetAllFollowings";
        //GetAllFollowers
        public static readonly string GetAllFollowers = "api/Account/GetAllFollowers";


        //Ad
        //Post Data
        public static readonly string PostAd = "api/Ad/PostAd";
        //Edit Data
        public static readonly string EditAd = "api/Ad/EditAd";
        //Delete Ad
        public static readonly string DeleteAd = "api/Ad/DeleteAd";
        //Get Ad By Id
        public static readonly string GetAdById = "api/Ad/GetAdById";
        //Get My Ads
        public static readonly string GetMyAds = "api/Ad/GetMyAds";
        //Get My Ads
        public static readonly string GetAllAdByUser = "api/Ad/GetAllAdByUser";
        //Get Popular Ads
        public static readonly string GetPopularAds = "api/Ad/GetPopularAds";
        //Get Recent Ads
        public static readonly string GetRecentAds = "api/Ad/GetRecentAds";
        //Get featured Ads
        public static readonly string GetFeaturedAds = "api/Ad/GetFeaturedAds";
        //MakeAdFeatured
        public static readonly string MakeAdFeatured = "api/Ad/MakeAdFeatured";
        //RemoveAdFromFeatured
        public static readonly string RemoveAdFromFeatured = "api/Ad/RemoveAdFromFeatured";
        //PauseAd
        public static readonly string PauseAd = "api/Ad/PauseAd";
        //UnpauseAd
        public static readonly string UnpauseAd = "api/Ad/UnpauseAd";
        //UnpauseAd
        public static readonly string MakeDeal = "api/Ad/MakeDeal";

        //Update Ad Views
        public static readonly string UpdateViews = "api/Ad/UpdateViews";
        //Renew Ad - Update EndOn
        public static readonly string RenewAd = "api/Ad/RenewAd";
        //GetAdCreator
        public static readonly string GetAdCreator = "api/Ad/GetAdCreator";


        //Category
        //Post Category
        public static readonly string PostCategory = "api/Category/PostCategoryAndSub";
        //Delete Category
        public static readonly string DeleteCategory = "api/Category/DeleteCategory";
        //Get Categories
        public static readonly string GetCategoriesAndSub = "api/Category/GetCategoriesAndSub";
        //Edit Category
        public static readonly string EditCategoryAndSub = "api/Category/EditCategoryAndSub";
        //Get Category By Id
        public static readonly string GetCategoriesAndSubById = "api/Category/GetCategoriesAndSubById";
        //Get Ads By Sub Category
        public static readonly string GetAdsBySubCategory = "api/Category/GetAdsBySubCategory";


        //Search
        //Simple Search
        public static readonly string SimpleSearchRequest = "api/Search/SimpleSearchRequest";


        //Comment
        //PostComment
        public static readonly string PostComment = "api/Comment/PostComment";
        //Get all Ad comments
        public static readonly string GetAllAdComment = "api/Comment/GetAllAdComments";
        //DeleteComment
        public static readonly string DeleteComment = "api/Comment/DeleteComment";
    }
}