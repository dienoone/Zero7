using Microsoft.AspNetCore.Http;
using O7.API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O7.API.Handler
{
    public interface IImageHandler
    {
        Task<string> UploadImage(IFormFile file);
    }
    public class ImageHandler : IImageHandler
    {
        private readonly IImageWriter _imageWriter;
        public ImageHandler(IImageWriter imageWriter)
        {
            _imageWriter = imageWriter;
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            return await _imageWriter.UploadImage(file);
        }
    }
}
