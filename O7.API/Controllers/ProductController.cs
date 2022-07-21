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
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageHandler _imageHandler;
        private readonly AppURL _appURL;
        private readonly IWebHostEnvironment _environment;

        public ProductController(IOptions<AppURL> AppURL, IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment environment, IImageHandler imageHandler)
        {
            _appURL = AppURL.Value;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageHandler = imageHandler;
            _environment = environment;
        }

        #region Product:

        // api/Product => get All Products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var entities = await _unitOfWork.Products.GetProductsAsync(HttpContext.GetBusinessId());
            var dto = _mapper.Map<IEnumerable<ProductDto>>(entities);
            foreach (var product in dto)
            {
                var collection = await _unitOfWork.CollectionProducts.FindAsync(e => e.ProductId == product.Id);
                Collection queryCollection = null;
                if (collection != null)
                {
                    queryCollection = await _unitOfWork.Collections.FindAsync(e => e.Id == collection.CollectionId);
                }

                if (queryCollection != null)
                {
                    product.ColloectionId = queryCollection.Id;
                    product.CollectionName = queryCollection.Name;
                }
            }

            return Ok(dto);
        }

        // api/product/{productId} => get Singel Product
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProduct([FromRoute] int? productId)
        {
            if (productId != null)
            {
                // Return New Product :
                Product entity = await _unitOfWork.Products.GetProductAsync(productId, HttpContext.GetBusinessId());
                if (entity != null)
                {
                    var collection = await _unitOfWork.CollectionProducts.FindAsync(e => e.ProductId == productId);
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
                return NotFound("This Product Doesn't Exist !!");
            }
            return NotFound("This Product Doesn't Exist !!");
        }

        // api/product => Create new Product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] AddProductsDto model)
        {
            if (ModelState.IsValid)
            {
                string businessId = HttpContext.GetBusinessId();
                Style style = await _unitOfWork.Styles.FindAsync(e => e.Id == model.StyleId && e.BusinessId == businessId);
                if (style != null)
                {
                    ProductType type = await _unitOfWork.ProductTypes.FindAsync(e => e.Id == model.TypeId && e.BusinessId == businessId);
                    if (type != null)
                    {
                        Product isExist = await _unitOfWork.Products.FindAsync(e => e.Name == model.Name && e.BusinessId == businessId);
                        if (isExist == null)
                        {
                            Product newProduct = new Product
                            {
                                Name = model.Name,
                                Description = model.Description,
                                Price = model.Price,
                                StyleId = model.StyleId,
                                ProductTypeId = model.TypeId,
                                BusinessId = businessId,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = HttpContext.GetUserId(),
                                IsDeleted = false
                            };
                            Product addedProduct = await _unitOfWork.Products.AddAsync(newProduct);

                            
                            if (addedProduct != null && await _unitOfWork.Complete())
                            {
                                if (model.CollectionId != null)
                                {
                                    Collection queryCollection = await _unitOfWork.Collections.FindAsync(e => e.Id == model.CollectionId);
                                    if (queryCollection != null)
                                    {
                                        CollectionProduct newCollecionProduct = new CollectionProduct
                                        {
                                            CollectionId = (int)model.CollectionId,
                                            ProductId = addedProduct.Id,
                                            BusinessId = businessId,
                                            CreatedAt = DateTime.UtcNow,
                                            CreatedBy = HttpContext.GetUserId(),
                                            IsDeleted = false

                                        };
                                        CollectionProduct addedCollectionProduct = await _unitOfWork.CollectionProducts.AddAsync(newCollecionProduct);
                                        if (addedCollectionProduct == null)
                                        {
                                            _unitOfWork.Products.Delete(addedProduct);
                                            return BadRequest("Error In Assign Product To Colleciton !!");
                                        }
                                    }
                                    else
                                    {
                                        _unitOfWork.Products.Delete(addedProduct);
                                        return NotFound("This Collection Doesn't Exist !!");
                                    }
                                }

                                if (model.Colors != null)
                                {
                                    foreach (var productColor in model.Colors)
                                    {
                                        Color color = await _unitOfWork.Colors.FindAsync(e => e.Id == productColor.ColorId && e.BusinessId == businessId);
                                        if (color != null)
                                        {
                                            ProductColor newProductColor = new ProductColor
                                            {
                                                ProductId = newProduct.Id,
                                                ColorId = color.Id,
                                                IsActive = false,
                                                BusinessId = businessId,
                                                CreatedAt = DateTime.UtcNow,
                                                CreatedBy = HttpContext.GetUserId(),
                                                IsDeleted = false
                                            };
                                            ProductColor addedProductColor = await _unitOfWork.ProductColors.AddAsync(newProductColor);

                                            if (addedProductColor != null && await _unitOfWork.Complete())
                                            {
                                                if (productColor.Photos != null)
                                                {
                                                    // Photos: 
                                                    foreach (var file in productColor.Photos)
                                                    {
                                                        if (file != null)
                                                        {
                                                            string photo = null;
                                                            if (file.Length > 0)
                                                            {
                                                                photo = await _imageHandler.UploadImage(file);
                                                                if (photo == null)
                                                                {
                                                                    _unitOfWork.Products.Delete(addedProduct);
                                                                    return BadRequest(file);
                                                                }
                                                                else
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
                                                                    if (addedPhoto == null && !await _unitOfWork.Complete())
                                                                    {
                                                                        _unitOfWork.Products.Delete(addedProduct);
                                                                        return BadRequest(file);
                                                                    }

                                                                }
                                                            }

                                                        }
                                                    }
                                                }

                                                // Sizes:
                                                if (productColor.Sizes != null)
                                                {
                                                    foreach (var productColorSize in productColor.Sizes)
                                                    {
                                                        Size size = await _unitOfWork.Sizes.FindAsync(e => e.Id == productColorSize.SizeId);
                                                        if (size != null)
                                                        {
                                                            ProductColorSize newProductColorSize = new ProductColorSize
                                                            {
                                                                ProductColorId = addedProductColor.Id,
                                                                SizeId = size.Id,
                                                                IsActive = false,
                                                                Quantity = productColorSize.Quantity,
                                                                BusinessId = businessId,
                                                                CreatedAt = DateTime.UtcNow,
                                                                CreatedBy = HttpContext.GetUserId(),
                                                                IsDeleted = false
                                                            };
                                                            ProductColorSize addedProductColorSize = await _unitOfWork.ProductColorSizes.AddAsync(newProductColorSize);

                                                            if (addedProductColorSize == null && !await _unitOfWork.Complete())
                                                            {
                                                                _unitOfWork.Products.Delete(addedProduct);
                                                                return BadRequest("Error in Add Size For Product !!");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            _unitOfWork.Products.Delete(addedProduct);
                                                            return BadRequest("This Size Doesn't Exist !!");
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                _unitOfWork.Products.Delete(addedProduct);
                                                return BadRequest("Error In Add Colors For Product !!");
                                            }

                                        }
                                        else
                                        {
                                            _unitOfWork.Products.Delete(addedProduct);
                                            return BadRequest("This Color Doesn't Exist !!");
                                        }

                                    }

                                } // modle.Colors
                            }
                            else
                            {
                                _unitOfWork.Products.Delete(addedProduct);
                                return BadRequest("Error In Add Product !!");
                            }

                            // Return New Product :
                            if (await _unitOfWork.Complete())
                            {
                                Product entity = await _unitOfWork.Products.GetProductAsync(addedProduct.Id, businessId);
                                var collection = await _unitOfWork.CollectionProducts.FindAsync(e => e.ProductId == addedProduct.Id);
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

                        }
                        return BadRequest("This Product Is Already Exist !!");

                    }
                    return NotFound("This Type Dosen't Exist !!");

                }
                return NotFound("This Style Doesn't Exist  !!");

            }
            return BadRequest(ModelState);

        }

        // api/product/{productId} => Edit Product
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int? productId, [FromBody] UpdateProductsDto model)
        {
            if (productId != null)
            {
                if (ModelState.IsValid)
                {
                    ProductType type = await _unitOfWork.ProductTypes.FindAsync(e => e.Id == model.TypeId);
                    if (type != null)
                    {
                        Style style = await _unitOfWork.Styles.FindAsync(e => e.Id == model.StyleId);
                        if (style != null)
                        {
                            Product queryProduct = await _unitOfWork.Products.FindAsync(e => e.Id == productId);
                            if (queryProduct != null)
                            {
                                queryProduct.Name = model.Name;
                                queryProduct.Description = model.Description;
                                queryProduct.Price = model.Price;
                                queryProduct.StyleId = style.Id;
                                queryProduct.ProductTypeId = type.Id;

                                if (model.Colors != null)
                                {
                                    foreach (var color in model.Colors)
                                    {
                                        if (color != null)
                                        {
                                            Color isColorExist = await _unitOfWork.Colors.FindAsync(e => e.Id == color.ColorId);
                                            if (isColorExist != null)
                                            {
                                                ProductColor queryProductColor = await _unitOfWork.ProductColors.FindAsync(e => e.Id == color.Id);
                                                if (queryProductColor != null)
                                                {
                                                    queryProductColor.IsActive = color.IsActive;
                                                    queryProductColor.ColorId = color.ColorId;

                                                    if (color.Photos != null)
                                                    {
                                                        foreach (var photo in color.Photos)
                                                        {
                                                            ProductColorImage queryPhoto = await _unitOfWork.ProductColorImages.FindAsync(e => e.Id == photo.Id);
                                                            if (queryPhoto != null)
                                                            {
                                                                queryPhoto.IsActive = photo.IsActive;
                                                            }
                                                            else
                                                            {
                                                                return BadRequest("This Product Color Image Doesn't Exist !!");
                                                            }
                                                        }
                                                    }

                                                    if (color.Sizes != null)
                                                    {
                                                        foreach (var colorSize in color.Sizes)
                                                        {
                                                            Size size = await _unitOfWork.Sizes.FindAsync(e => e.Id == colorSize.SizeId);
                                                            if (size != null)
                                                            {
                                                                ProductColorSize queryProductColorSzie = await _unitOfWork.ProductColorSizes.FindAsync(e => e.Id == colorSize.Id);
                                                                if (queryProductColorSzie != null)
                                                                {
                                                                    queryProductColorSzie.SizeId = colorSize.SizeId;
                                                                    queryProductColorSzie.Quantity = colorSize.Quantity;
                                                                    queryProductColorSzie.IsActive = colorSize.IsActive;
                                                                }
                                                                else
                                                                {
                                                                    return BadRequest("This Product Color Size Doesn't Exist !!");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                return BadRequest("This Size Doesn't Exist !!");
                                                            }
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    return BadRequest("This Product Color Doesn't Exist !!");
                                                }
                                            }
                                            else
                                            {
                                                return BadRequest("This Color Doesn't Exist !!");
                                            }
                                        }
                                        else
                                        {
                                            return BadRequest("This Product Color Doesn't Exist !!");
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
                                return BadRequest("Error In Edit This Product !!");
                            }
                            return NotFound("This Product Doesn't Exist !!");
                        }
                        return NotFound("This Style Doesn't Exist !!");
                    }
                    return NotFound("This Type Doesn't Exist !!");
                }
                return BadRequest(ModelState);
            }
            return NotFound("This Product Doesn't Exist !!");

        }

        // api/product/{productId} => Delete Product
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int? productId)
        {
            if (productId != null)
            {
                Product queryProduct = await _unitOfWork.Products.GetProductAsync(productId, HttpContext.GetBusinessId());
                if (queryProduct != null)
                {
                    queryProduct.IsDeleted = true;

                    if (queryProduct.ProductColors != null)
                    {
                        foreach (var productColor in queryProduct.ProductColors)
                        {
                            productColor.IsDeleted = true;

                            if (productColor.ProductColorImages != null)
                            {
                                foreach (var photo in productColor.ProductColorImages)
                                {
                                    var wwwroot = _environment.WebRootPath;
                                    var guid = photo.Photo.Remove(0, (_appURL.AppUrl.Length + 8) - 1).ToString();
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

                        }
                    }

                    if (await _unitOfWork.Complete())
                    {
                        return NoContent();
                    }
                }
                return BadRequest("This Product Doesn't Exist !!");
            }
            return BadRequest("This Product Doesn't Exist !!");
        }

        #endregion


    }
}
