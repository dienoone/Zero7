using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.ViewModels.O7ViewModels
{
    public class AddProductColorDto
    {
        [Required]
        public int ColorId { get; set; }
        public List<IFormFile> Photos { get; set; }

    }
    public class UpdateProductColorDto
    {
        public int ColorId { get; set; }
        public bool IsActive { get; set; }
        public List<ProductColorImageDto> Photos { get; set; }
        public List<ProductColorImageDto> DeletedPhotos { get; set; }

    }
    public class UpdateProductPhotoDto
    {
        public List<IFormFile> Photos { get; set; }
    }
}
