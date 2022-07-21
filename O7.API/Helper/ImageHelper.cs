using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.API.Helper
{
    public interface IImageWriter
    {
        Task<string> UploadImage(IFormFile file);
    }

    public class ImageWriter : IImageWriter
    {

        public async Task<string> UploadImage(IFormFile file)
        {
            if (await CheckIfImageFile(file))
            {
                return await WriterFile(file);
            }
            return null;
        }

        private async Task<string> WriterFile(IFormFile file)
        {
            string fileName = null;
            try
            {
                var extension = new StringBuilder(".").Append(file.FileName.Split(".")[file.FileName.Split(".").Length - 1]);
                fileName = new StringBuilder(Guid.NewGuid().ToString()).Append(extension).ToString();
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                using (var bits = new FileStream(path, FileMode.Create)) await file.CopyToAsync(bits);
            }
            catch (Exception ex)
            {
                return null;
            }
            return fileName;
        }

        private async Task<bool> CheckIfImageFile(IFormFile file)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                // Upload the file if less than 2 MB
                //if (ms.Length < 2097152)
                //{
                //    return false;
                //}
                fileBytes = ms.ToArray();
            }
            return WriteHelper.GetImageFormat(fileBytes) != WriteHelper.ImageFormat.unknown;
        }

    }

    public class WriteHelper
    {
        public enum ImageFormat
        {
            bmp,
            jpeg,
            jpeg2,
            gif,
            tiff,
            tiff2,
            png,
            unknown
        };

        internal static ImageFormat GetImageFormat(byte[] fileBytes)
        {
            var bmp = Encoding.ASCII.GetBytes("BM");
            var gif = Encoding.ASCII.GetBytes("GIF");
            var png = new byte[] { 137, 80, 78, 71 }; // headers bytes
            var tiff = new byte[] { 73, 73, 42 }; // headers bytes
            var tiff2 = new byte[] { 77, 77, 42 }; // headers bytes
            var jpeg = new byte[] { 255, 216, 255, 224 }; // headers bytes
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // headers bytes

            if (bmp.SequenceEqual(fileBytes.Take(bmp.Length))) return ImageFormat.bmp;
            if (gif.SequenceEqual(fileBytes.Take(gif.Length))) return ImageFormat.gif;
            if (png.SequenceEqual(fileBytes.Take(png.Length))) return ImageFormat.png;
            if (tiff.SequenceEqual(fileBytes.Take(tiff.Length))) return ImageFormat.tiff;
            if (tiff.SequenceEqual(fileBytes.Take(tiff.Length))) return ImageFormat.tiff;
            if (tiff2.SequenceEqual(fileBytes.Take(tiff2.Length))) return ImageFormat.tiff2;
            if (jpeg.SequenceEqual(fileBytes.Take(jpeg.Length))) return ImageFormat.jpeg;
            if (jpeg2.SequenceEqual(fileBytes.Take(jpeg2.Length))) return ImageFormat.jpeg2;
            return ImageFormat.unknown;

        }

    }
}
