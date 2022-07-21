using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.ViewModels.O7ViewModels
{
    public class AddProductsDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int StyleId { get; set; }
        [Required]
        public int TypeId { get; set; }
        public int? CollectionId { get; set; }

        public List<AddProductColorsDto> Colors { get; set; }

    }
    public class AddProductColorsDto
    {
        [Required]
        public int ColorId { get; set; }
        public List<IFormFile> Photos { get; set; }
        public List<AddProudctColorSizesDto> Sizes { get; set; }
    }
    public class AddProudctColorSizesDto
    {
        [Required]
        public int SizeId { get; set; }
        public int Quantity { get; set; }
    }

    // -----------------------------------------------------
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int StyleId { get; set; }
        public string StyleName { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public int ColloectionId { get; set; }
        public string CollectionName { get; set; }
        public List<ProductColorDto> Colors { get; set; }

    }

    public class ProductColorDto
    {
        public int Id { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public bool IsActive { get; set; }

        public List<ProductColorImageDto> Photos { get; set; }
        public List<ProductColorSizeDto> Sizes { get; set; }
    }
    public class ProductColorImageDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Photo { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
    public class ProductColorSizeDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int SizeId { get; set; }
        public string SizeName { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
    // -------------------------------------------------------
    public class UpdateProductsDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int StyleId { get; set; }
        [Required]
        public int TypeId { get; set; }
        public List<UpdateProductColorsDto> Colors { get; set; }

    }
    public class UpdateProductColorsDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ColorId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public List<ProductColorImageDto> Photos { get; set; }
        public List<UpdateProudctColorSizeDto> Sizes { get; set; }

    }
    public class UpdateProductPhotosDto
    {
        public List<IFormFile> Photos { get; set; }
    }
}
