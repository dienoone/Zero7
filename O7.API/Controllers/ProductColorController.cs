using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using O7.API.Extentions;
using O7.API.Handler;
using O7.Core;
using O7.Core.Consts;
using O7.Core.Models.O7Models.Main;
using O7.Core.ViewModels.O7ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace O7.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductColorController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageHandler _imageHandler;
        private readonly AppURL _appURL;
        private readonly IWebHostEnvironment _environment;

        public ProductColorController(IOptions<AppURL> AppURL, IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment environment, IImageHandler imageHandler)
        {
            _appURL = AppURL.Value;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageHandler = imageHandler;
            _environment = environment;
        }

        #region ProductColors:

        // api/ProductColor/{ProductId} => Create New ProductColor
        [HttpPost("{ProductId}")]
        public async Task<IActionResult> CreateProductColor([FromRoute] int? ProductId, [FromForm] AddProductColorsDto model)
        {
            if (ProductId != null)
            {
                if (ModelState.IsValid)
                {
                    Product queryProduct = await _unitOfWork.Products.FindAsync(e => e.Id == ProductId);
                    if (queryProduct != null)
                    {
                        Color color = await _unitOfWork.Colors.FindAsync(e => e.Id == model.ColorId && e.BusinessId == HttpContext.GetBusinessId());
                        if (color != null)
                        {
                            ProductColor newProductColor = new ProductColor
                            {
                                ProductId = queryProduct.Id,
                                ColorId = color.Id,
                                IsActive = false,
                                BusinessId = HttpContext.GetBusinessId(),
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = HttpContext.GetUserId(),
                                IsDeleted = false
                            };
                            ProductColor addedProductColor = await _unitOfWork.ProductColors.AddAsync(newProductColor);

                            if (addedProductColor != null && await _unitOfWork.Complete())
                            {
                                if (model.Photos != null)
                                {
                                    // Photos: 
                                    foreach (var file in model.Photos)
                                    {
                                        if (file != null)
                                        {
                                            string photo = null;
                                            if (file.Length > 0)
                                            {
                                                photo = await _imageHandler.UploadImage(file);
                                                if (photo != null)
                                                {
                                                    ProductColorImage newProductColorImage = new ProductColorImage
                                                    {
                                                        Photo = _appURL.AppUrl + "images/" + photo,
                                                        IsActive = false,
                                                        ProductColorId = addedProductColor.Id,
                                                        IsDeleted = false,
                                                        CreatedAt = DateTime.UtcNow,
                                                        CreatedBy = HttpContext.GetUserId(),
                                                        BusinessId = HttpContext.GetBusinessId()
                                                    };
                                                    ProductColorImage addedPhoto = await _unitOfWork.ProductColorImages.AddAsync(newProductColorImage);
                                                    if (addedPhoto == null)
                                                    {
                                                        _unitOfWork.ProductColors.Delete(addedProductColor);
                                                        return BadRequest(file);
                                                    }

                                                }
                                                else
                                                {
                                                    _unitOfWork.ProductColors.Delete(addedProductColor);
                                                    return BadRequest(file);

                                                }

                                            }

                                        }

                                    }
                                }

                                if (model.Sizes != null)
                                {
                                    foreach (var newSize in model.Sizes)
                                    {
                                        Size size = await _unitOfWork.Sizes.FindAsync(e => e.Id == newSize.SizeId);
                                        if (size != null)
                                        {
                                            ProductColorSize newProductColorSize = new ProductColorSize
                                            {
                                                SizeId = newSize.SizeId,
                                                Quantity = newSize.Quantity,
                                                IsActive = true,
                                                ProductColorId = addedProductColor.Id,
                                                BusinessId = HttpContext.GetBusinessId(),
                                                CreatedAt = DateTime.UtcNow,
                                                CreatedBy = HttpContext.GetUserId(),
                                                IsDeleted = false
                                            };
                                            ProductColorSize addedProductColorSize = await _unitOfWork.ProductColorSizes.AddAsync(newProductColorSize);
                                            if (addedProductColorSize == null)
                                            {
                                                _unitOfWork.ProductColors.Delete(addedProductColor);
                                                return BadRequest("Error in Add Product Color Size !!!!");
                                            }
                                        }
                                        else
                                        {
                                            _unitOfWork.ProductColors.Delete(addedProductColor);
                                            return BadRequest("This Size Doesn't Exist !!");
                                        }
                                    }
                                }

                                if (await _unitOfWork.Complete())
                                {
                                    Product entity = await _unitOfWork.Products.GetProductAsync(queryProduct.Id, HttpContext.GetBusinessId());
                                    var collection = await _unitOfWork.CollectionProducts.FindAsync(e => e.ProductId == queryProduct.Id);
                                    Collection queryCollection = null;
                                    var dto = _mapper.Map<ProductDto>(entity);
                                    if (collection != null)
                                    {
                                        queryCollection = await _unitOfWork.Collections.FindAsync(e => e.Id == collection.CollectionId);
                                    }
                                    if (queryCollection != null)
                                    {
                                        dto.ColloectionId = queryCollection.Id;
                                        dto.CollectionName = queryCollection.Name;
                                    }
                                    return Ok(dto);
                                }
                                return BadRequest("Error In Add Product Color !!");
                            }
                            return BadRequest("Error In Add ProductColor !!");
                        }
                        return NotFound("This Color Doesn't Exist !!");
                    }
                    return NotFound("This Product Doesn't Exist !!");
                }
                return BadRequest(ModelState);
            }
            return NotFound("This Product Doesn't Exist !!");
        }

        // api/ProductColor/{ProductColorId} => Delete ProductColor
        [HttpDelete("{ProductColorId}")]
        public async Task<IActionResult> DeleteProductColor([FromRoute] int? ProductColorId)
        {
            if (ProductColorId != null)
            {
                ProductColor productColor = await _unitOfWork.ProductColors.GetProductColorAsync(ProductColorId, HttpContext.GetBusinessId());
                if (productColor != null)
                {
                    productColor.IsDeleted = true;

                    if (productColor.ProductColorImages != null)
                    {
                        foreach (var photo in productColor.ProductColorImages)
                        {
                            var wwwroot = _environment.WebRootPath;
                            string guid = photo.Photo.Remove(0, (_appURL.AppUrl.Length + 8) - 1).ToString();
                            var path = Path.Combine(wwwroot, "images", guid);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                                _unitOfWork.ProductColorImages.Delete(photo);
                            }
                        }
                    }
                    if (productColor.ProductColorSizes != null)
                    {
                        foreach (var productColorSize in productColor.ProductColorSizes)
                        {
                            productColorSize.IsDeleted = true;
                        }
                    }
                    return NoContent();
                }
                return NotFound("This Product Color Doesn't Exist !!");
            }
            return NotFound("This Product Color Doesn't Exist !!");
        }

        // api/ProductColor/photo/{ProductColorId} => Create new ProductColorImage
        [HttpPost("photo/{ProductColorId}")]
        public async Task<IActionResult> UpdateProductColorPhoto([FromRoute] int? ProductColorId, [FromForm] UpdateProductPhotoDto model)
        {
            if (ProductColorId != null)
            {
                if (ModelState.IsValid)
                {
                    ProductColor queryProductColor = await _unitOfWork.ProductColors.FindAsync(e => e.Id == ProductColorId);
                    if (queryProductColor != null)
                    {
                        if (model.Photos != null)
                        {
                            // Photos: 
                            foreach (var file in model.Photos)
                            {
                                if (file != null)
                                {
                                    string photo = null;
                                    if (file.Length > 0)
                                    {
                                        photo = await _imageHandler.UploadImage(file);
                                        if (photo != null)
                                        {
                                            ProductColorImage newProductColorImage = new ProductColorImage
                                            {
                                                Photo = _appURL.AppUrl + "images/" + photo,
                                                IsActive = false,
                                                ProductColorId = queryProductColor.Id,
                                                IsDeleted = false,
                                                CreatedAt = DateTime.UtcNow,
                                                CreatedBy = HttpContext.GetUserId(),
                                                BusinessId = HttpContext.GetBusinessId()
                                            };
                                            ProductColorImage addedPhoto = await _unitOfWork.ProductColorImages.AddAsync(newProductColorImage);
                                            if (addedPhoto == null)
                                            {
                                                return BadRequest(file);
                                            }

                                        }
                                        else
                                        {
                                            return BadRequest(file);

                                        }

                                    }

                                }

                            }
                        }

                        if (await _unitOfWork.Complete())
                        {
                            Product entity = await _unitOfWork.Products.GetProductAsync(queryProductColor.ProductId, HttpContext.GetBusinessId());
                            var collection = await _unitOfWork.CollectionProducts.FindAsync(e => e.ProductId == queryProductColor.Id);
                            Collection queryCollection = null;
                            var dto = _mapper.Map<ProductDto>(entity);
                            if (collection != null)
                            {
                                queryCollection = await _unitOfWork.Collections.FindAsync(e => e.Id == collection.CollectionId);
                            }
                            if (queryCollection != null)
                            {
                                dto.ColloectionId = queryCollection.Id;
                                dto.CollectionName = queryCollection.Name;
                            }
                            return Ok(dto);
                        }
                        return BadRequest("Error in Add Photo !!");
                    }
                    return BadRequest("This Product Color Doesn't Exist !!");
                }
                return BadRequest(ModelState);
            }
            return NotFound("This Product Color Doesn't Exist !!");
        }

        #endregion
    }
}
