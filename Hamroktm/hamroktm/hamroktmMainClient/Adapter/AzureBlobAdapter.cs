using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace hamroktmMainClient.Adapter
{
    public class AzureBlobAdapter
    {
        public CloudBlobClient CloudBlobContainer { get; set; }

        public int CacheMaxAge { get; set; }

        public AzureBlobAdapter(string storageAccountName, string storageAccountKey)
        {
            CloudBlobContainer = SetUpBlobClient(storageAccountName, storageAccountKey);

            CacheMaxAge = 8600;
        }

        private CloudBlobClient SetUpBlobClient(string storageAccountName, string storageAccountKey)
        {
            var connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            return cloudBlobClient;
        }

        public void RunAtAppStartup(string storageAccountName, string storageAccountKey, string containerName)
        {
            var startCloudBlobClient = SetUpBlobClient(storageAccountName, storageAccountKey);

            var startCloudBlobContainer = startCloudBlobClient.GetContainerReference(containerName);


            startCloudBlobContainer.CreateIfNotExists();

            var permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };

            startCloudBlobContainer.SetPermissions(permissions);
        }


        //Upload a Single Stream File
        public HttpStatusCodeResult UploadFromStream(MemoryStream memoryStreamFile, string filename, string contentType,
            string containerName)
        {
            try
            {
                var container = GetBlobContainer(containerName, filename);

                var blob = container.GetBlockBlobReference(filename);

                blob.Properties.ContentType = contentType;

                memoryStreamFile.Position = 0;
                blob.UploadFromStream(memoryStreamFile);

                return new HttpStatusCodeResult(HttpStatusCode.OK);

            }
            catch(Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,ex.Message);
            }
        }


        //Upload a Single File

        public HttpStatusCodeResult UploadFromFileBase(HttpPostedFileBase file, string containerName)
        {
            try
            {
                var ms = new MemoryStream();
                var filename = new FileInfo(file.FileName).Name; //strips the C:/documents/...

                file.InputStream.CopyTo(ms);

                var response = UploadFromStream(ms, filename, file.ContentType, containerName);
                if (response.StatusCode == (int) HttpStatusCode.OK)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }

                throw new Exception("Error Uploading the File " + filename + response.StatusDescription);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest,ex.Message);
            }
        }

        public HttpStatusCodeResult DeleteFile(string fileName, string containerName)
        {
            try
            {
                var container = GetBlobContainer(containerName, fileName);

                var blob = container.GetBlockBlobReference(fileName);

                blob.DeleteIfExists();

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        private CloudBlobContainer GetBlobContainer(string containerName, string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            if (String.IsNullOrWhiteSpace(containerName))
            {
                containerName = GetContainerNameFromFileExtension(fileExtension);
            }

            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new Exception("Cannot upload files with extension: " + fileExtension);
            }

            var container = CloudBlobContainer.GetContainerReference(containerName);

            return container;
        }

        private string GetContainerNameFromFileExtension(string fileExtension)
        {
            var containerName = "";
            switch (fileExtension.ToLower())
            {
                case ".jpg":
                case ".gif":
                case ".bmp":
                case ".png":
                    containerName = "hamroktmcontainer/Images";
                    break;
            }
            return containerName;

        }

        public HttpStatusCodeResult Upload(IEnumerable<HttpPostedFileBase> files, string containerName = "")
        {
            var uploadfiles = files.ToList();

            var fileTotalCount = uploadfiles.Count();

            var fileCount = 0;

            var response = (int) HttpStatusCode.OK;
            var message = string.Empty;
            while (response == (int) HttpStatusCode.OK && fileCount < fileTotalCount)
            {
                var file = uploadfiles[fileCount];
                var resp = UploadFromFileBase(file, containerName);

                response = resp.StatusCode;
                message = resp.StatusDescription;

                fileCount++;
            }

            return response == (int) HttpStatusCode.OK
                ? new HttpStatusCodeResult(HttpStatusCode.OK)
                : new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);

        }
    }
}