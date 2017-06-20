using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using hamroktmMainClient.ViewModels;
using contracts.Models;
using hamroktmMainClient.Adapter;
using hamroktm.Helpers;
using hamroktmMainClient.Proxies;
using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace hamroktmMainClient.Controllers
{
    [TokenAuthorize]
    public class AdController : BaseController
    {
        private static readonly string StorageAccount = ConfigurationManager.AppSettings["BlobStorageAccountName"];
        private static readonly string StorageKey = ConfigurationManager.AppSettings["BlobStorageAccountKey"];

        private readonly IAdProxy _proxy;

        public AdController(IAdProxy proxy)
        {
            _proxy = proxy;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Index(int adId)
        {
            try
            {
                var model = await GetAdById(adId);
                if (model != null)
                {
                    return View(model);
                }
                return HttpNotFound();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetPopularAds()
        {
            try
            {
                var response = await _proxy.GetPopularAds();
                if (response != null)
                {
                    response = response.Take(5);
                }
                return PartialView("Partial/Ad/AdListingsPartial", response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetFeaturedAds()
        {
            try
            {
                var response = await _proxy.GetFeaturedAds();
                return PartialView("Partial/Ad/FeaturedadcarouselPartial", response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetRecentAds()
        {
            try
            {
                var response = await _proxy.GetRecentAds();
                //converting json data to list of adcontract... need to update this after implementing react.
                if (response != null)
                {
                    response = response.Take(5);
                }
                return PartialView("Partial/Ad/AdListingsPartial", response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult PostAd()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PostAd(AdContract model, IEnumerable<HttpPostedFileBase> imagestoUpload = null)
        {
            try
            {
                if (model.Tags.Any())
                {
                    model.Tags = model.Tags[0].Split(',').ToList();
                }
                if (ModelState.IsValid)
                {
                    bool success = true;
                    int i = 0;
                    var fileName = DateTime.Now.ToString("yyyyMMddhhmmss");

                    //resizing images and uploading
                    foreach (var image in imagestoUpload)
                    {
                        if (image != null)
                        {
                            if (image.ContentLength > 0)
                            {
                                if (IsImage(image))
                                {
                                    if (i == 0)
                                    {
                                        //resize
                                        var newSmallImage = ResizeImageToSmall(image, fileName);
                                        var newXSmallImage = ResizeImageToXSmall(image, fileName);

                                        //upload
                                        var result1 = UploadFile(newSmallImage);
                                        var result2 = UploadFile(newXSmallImage);
                                        i++;
                                    }

                                    //resize
                                    var newLargeImage = ResizeImageToLarge(image, fileName);
                                    //var updatedFileName = newLargeImage.FileName.Replace("Large_", "");
                                    var updatedFileName = newLargeImage.FileName.Remove(0, 6);
                                    model.Images.Add(updatedFileName);

                                    //upload
                                    var result3 = UploadFile(newLargeImage);
                                }
                                else
                                {
                                    success = false;
                                    //file submitted is not image file.
                                }
                            }
                        }
                    }

                    //data upload to database
                    if (success)
                    {
                        model.CreatedBy = GetUserName();
                        model.CreatedOn = DateTime.Now;
                        model.UpdatedOn = DateTime.Now;
                        model.UpdatedBy = GetUserName();
                        model.Views = 0;
                        model.EndOn = model.StartOn.AddDays(30);
                        if (model.StartOn > DateTime.Now)
                        {
                            model.Status = "Onhold";
                        }
                        else
                        {
                            model.Status = "Live";
                        }

                        var token = GetToken();
                        var response = await _proxy.PostAd(model, token);
                        if (response.IsSuccessStatusCode)
                        {
                            ViewData["SuccessResult"] = "Ad has been Posted!";
                        }
                        return View("Manage");
                    }
                }
                ViewData["ErrorResult"] = "Please fill the required fields appropriately!";
                return View(model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ActionResult> GetEditAdPartial(int id)
        {
            try
            {
                var responseModel = await GetAdById(id);
                return PartialView("Partial/Ad/EditAdPartial", responseModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditAd(AdContract model, IEnumerable<HttpPostedFileBase> imagestoUpload = null)
        {
            try
            {
                var response = await EditAdTask(model, imagestoUpload);
                if (response.StatusCode == 200)
                {
                    ViewData["SuccessResult"] = "Ad has been updated!";
                    return View("Manage");
                }

                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Some Error");
                ViewData["ErrorResult"] = "Could not update the Ad. Please try again!";
                return View("Manage");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<HttpStatusCodeResult> EditAdTask(AdContract model, IEnumerable<HttpPostedFileBase> imagestoUpload = null)
        {
            try
            {
                if (model.Tags.Count > 3)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Number of Tags exceeded. Maximum : 3");
                }

                if (ModelState.IsValid)
                {
                    bool success = true;
                    int i = 0;
                    int count = 0;
                    var fileName = DateTime.Now.ToString("yyyyMMddhhmmss");

                    if (imagestoUpload != null)
                    {
                        //resizing images and uploading
                        foreach (var image in imagestoUpload)
                        {
                            if (image != null)
                            {
                                if (image.ContentLength > 0)
                                {
                                    if (IsImage(image))
                                    {
                                        if (i == 0)
                                        {
                                            //resize
                                            var newSmallImage = ResizeImageToSmall(image, fileName);
                                            var newXSmallImage = ResizeImageToXSmall(image, fileName);

                                            //upload
                                            var result1 = UploadFile(newSmallImage);
                                            var result2 = UploadFile(newXSmallImage);
                                            i++;
                                        }

                                        //resize
                                        var newLargeImage = ResizeImageToLarge(image, fileName);
                                        //var updatedFileName = newLargeImage.FileName.Replace("Large_", "");
                                        var updatedFileName = newLargeImage.FileName.Remove(0, 6);
                                        if (count + 1 <= model.Images.Count)
                                        {
                                            model.Images[count] = updatedFileName;
                                        }
                                        else
                                        {
                                            model.Images.Add(updatedFileName);
                                        }
                                        //upload
                                        var result3 = UploadFile(newLargeImage);
                                    }
                                    else
                                    {
                                        success = false;
                                        //file submitted is not image file.
                                    }
                                }
                            }
                            count++;
                        }
                    }

                    //data upload to database
                    if (success)
                    {
                        model.Images=model.Images.Where(x => !string.IsNullOrEmpty(x)).ToList();
                        model.UpdatedBy = User.Identity.GetUserName();
                        model.UpdatedOn = DateTime.Now;

                        var token = GetToken();
                        var response = await _proxy.EditAd(model, token);
                        if (response.IsSuccessStatusCode)
                        {

                            return new HttpStatusCodeResult(HttpStatusCode.OK, "Ad has been updated!");
                        }
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ad could not be updated!");
                    }
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Number of Tags exceeded. Maximum : 3");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAd(int id)
        {
            try
            {
                var token = GetToken();
                var response = await _proxy.DeleteAd(id, token);
                //if success delete associated images later
                return new HttpStatusCodeResult(response.StatusCode, "Deleted!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetMyAds()
        {
            try
            {
                var token = GetToken();
                var response = await _proxy.GetMyAds(token);
                return PartialView("Partial/Ad/MyAdsPartial", response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> UsersAdList(string userName)
        {
            try
            {
                var response = await _proxy.GetAllAdByUser(userName);
                ViewData["UserName"] = userName;
                return View(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<int> GetAllAdByUserCount(string userName)
        {
            try
            {
                var response = await _proxy.GetAllAdByUser(userName);
                return response.Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public async Task<string> GetAdCreator(int adId)
        {
            try
            {
                var response = await _proxy.GetAdCreator(adId);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        public async Task<AdContract> GetAdById(int id)
        {
            try
            {
                var response = await _proxy.GetAdById(id);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPatch]
        public async Task<ActionResult> UpdateViews(int id)
        {
            try
            {
                //if id not found in cookie
                var response = await _proxy.UpdateViews(id);
                return new HttpStatusCodeResult(response.StatusCode, "Updated View");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> MakeDeal(DealContract model = null)
        {
            try
            {
                if (ModelState.IsValid && model != null)
                {
                    var response = await _proxy.MakeDeal(model);
                    return new HttpStatusCodeResult(response.StatusCode, "Deal Made!");
                }
                //if success delete associated images
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Deal not Made!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPatch]
        public async Task<ActionResult> RenewAd(int id)
        {
            try
            {
                //if id not found in cookie
                var token = GetToken();
                var checkAd = await GetAdById(id);
                if (checkAd.Status == "Live" && checkAd.EndOn > DateTime.Now.AddDays(29))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error Renewing Ad! Please try after couple of days!");
                }

                var endOn = DateTime.Now.AddDays(30);
                var response = await _proxy.RenewAd(id, endOn, token);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error Renewing Ad! Please try again after some minutes");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPatch]
        public async Task<ActionResult> PauseAd(int id)
        {
            try
            {
                //if id not found in cookie
                var token = GetToken();
                var response = await _proxy.PauseAd(id, token);
                if (response.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(response.StatusCode, "Ad Paused");
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ad already Paused!");
                }
                return new HttpStatusCodeResult(response.StatusCode, "Error! Could not Pause Ad!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPatch]
        public async Task<ActionResult> UnpauseAd(int id)
        {
            try
            {
                //if id not found in cookie
                var token = GetToken();

                var response = await _proxy.UnpauseAd(id, token);
                if (response.IsSuccessStatusCode)
                {
                    return new HttpStatusCodeResult(response.StatusCode, "Ad Unpaused!");

                }
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ad not found!");
                }
                return new HttpStatusCodeResult(response.StatusCode, "Error! Could not Unpause Ad! Try again Later!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> AllRecentAds()
        {
            try
            {
                var response = await _proxy.GetRecentAds();
                return View(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> AllPopularAds()
        {
            try
            {
                var response = await _proxy.GetPopularAds();
                return View(response);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [AllowAnonymous]
        public ActionResult GetAdPageResult(List<AdContract> model)
        {
            return PartialView("Partial/Ad/AdListingsPartial", model);
        }

        public HttpPostedFileBase ResizeImageToSmall(HttpPostedFileBase file, string fileNameDateTime)
        {
            try
            {
                ImageUtilities smallImage = new ImageUtilities();

                var filename = new String(Path.GetFileName(file.FileName).Where(Char.IsLetterOrDigit).ToArray());
                var image = Image.FromStream(file.InputStream);
                double height = image.Height;
                double width = image.Width;

                if (height > width)
                {
                    double ratioX = 150 / height;
                    height = 150;
                    width = width * ratioX;

                }
                else if (width > height)
                {
                    double ratioY = 150 / width;
                    width = 150;
                    height = height * ratioY;
                }
                else
                {
                    width = 150;
                    height = 150;
                }

                int newWidth = (int)width;
                int newHeight = (int)height;

                var newImage = smallImage.ResizeImageWithQuality(image, newHeight, newWidth);

                image.Dispose();

                var newFileName = string.Format("{0}{1}{2}{3}", "Small_", fileNameDateTime, filename,".jpg");

                var response = ChangeToHttpFileBase(newImage, filename, newFileName);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public HttpPostedFileBase ResizeImageToXSmall(HttpPostedFileBase file, string fileNameDateTime)
        {
            try
            {
                ImageUtilities smallImage = new ImageUtilities();

                var filename = new String(Path.GetFileName(file.FileName).Where(Char.IsLetterOrDigit).ToArray());
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
                var newFileName = string.Format("{0}{1}{2}{3}", "XSmall_", fileNameDateTime, filename, ".jpg");

                var response = ChangeToHttpFileBase(newImage, filename, newFileName);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public HttpPostedFileBase ResizeImageToLarge(HttpPostedFileBase file, string fileNameDateTime)
        {
            try
            {
                ImageUtilities smallImage = new ImageUtilities();

                var filename = new String(Path.GetFileName(file.FileName).Where(Char.IsLetterOrDigit).ToArray());
                var image = Image.FromStream(file.InputStream);
                double height = image.Height;
                double width = image.Width;

                if (height > width)
                {
                    double ratioX = 500 / height;
                    height = 500;
                    width = width * ratioX;

                }
                else if (width > height)
                {
                    double ratioY = 500 / width;
                    width = 500;
                    height = height * ratioY;
                }
                else
                {
                    width = 500;
                    height = 500;
                }

                int newWidth = (int)width;
                int newHeight = (int)height;

                var newImage = smallImage.ResizeImageWithQuality(image, newHeight, newWidth);

                image.Dispose();
                var newFileName = string.Format("{0}{1}{2}{3}", "Large_", fileNameDateTime, filename, ".jpg");

                var response = ChangeToHttpFileBase(newImage, filename, newFileName);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static HttpStatusCodeResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                var blobAcess = new AzureBlobAdapter(StorageAccount, StorageKey);

                var files = new List<HttpPostedFileBase> { file };

                var response = blobAcess.Upload(files, "hamroktmcontainer/ProductImages");

                if (response.StatusCode == (int)HttpStatusCode.OK)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static MemoryFile ChangeToHttpFileBase(Bitmap newImage, string filename, string newFileName)
        {
            try
            {
                var format = ImageFormat.Jpeg;

                var mime = string.Format("Image/{0}", format);

                var memoryStream = new MemoryStream();

                newImage.Save(memoryStream, format);

                MemoryFile newFile = new MemoryFile(memoryStream, mime, newFileName);

                return newFile;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
    }
}