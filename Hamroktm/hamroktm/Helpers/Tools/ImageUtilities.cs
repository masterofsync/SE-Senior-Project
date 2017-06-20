using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

//resize the image to the specified height and width example
namespace hamroktm.Helpers
{
    public class ImageUtilities
    {
        public Bitmap ResizeImageWithQuality(Image image,int newHeight,int newWidth)
        {
            var newImage=new Bitmap(newWidth,newHeight);

            using (var graph = Graphics.FromImage(newImage))
            {
                graph.SmoothingMode = SmoothingMode.HighQuality;
                graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graph.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        private static MemoryFile ChangeToHttpFileBase(Bitmap newImage, string filename)
        {
            var format = ImageFormat.Jpeg;

            var mime = string.Format("Image/{0}", format);

            var memoryStream = new MemoryStream();

            newImage.Save(memoryStream, format);

            var newFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddhhmmss"), filename);

            MemoryFile newFile = new MemoryFile(memoryStream, mime, newFileName);

            return newFile;
        }
    }

    public class MemoryFile : HttpPostedFileBase
    {
        private readonly Stream _stream;
        private readonly string _contentType;
        private readonly string _fileName;

        public MemoryFile(Stream stream, string contentType, string fileName)
        {
            stream.Position = 0;
            _stream = stream;
            _contentType = contentType;
            _fileName = fileName;
        }

        public override int ContentLength
        {
            get { return (int)_stream.Length; }
        }

        public override string ContentType
        {
            get { return _contentType; }
        }

        public override string FileName
        {
            get { return _fileName; }
        }

        public override Stream InputStream
        {
            get { return _stream; }
        }

        public override void SaveAs(string filename)
        {
            using (var file = File.Open(filename, FileMode.CreateNew))
                _stream.CopyTo(file);
        }
    }
}