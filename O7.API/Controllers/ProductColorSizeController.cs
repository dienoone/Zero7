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
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace O7.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductColorSizeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductColorSizeController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #region ProductColorSzie:

        // api/ProductColorSize/{ProductColorId} => Create New ProductColorSize
        [HttpPost("{productColorId}")]
        public async Task<IActionResult> CreateProductColorSize([FromRoute] int? productColorId, [FromBody] AddProudctColorSizeDto model)
        {
            if (productColorId != null)
            {
                if (ModelState.IsValid)
                {
                    ProductColor queryProductColor = await _unitOfWork.ProductColors.FindAsync(e => e.Id == productColorId);
                    if (queryProductColor != null)
                    {
                        Size size = await _unitOfWork.Sizes.FindAsync(e => e.Id == model.SizeId);
                        if (size != null)
                        {
                            ProductColorSize newProductColorSize = new ProductColorSize
                            {
                                ProductColorId = queryProductColor.Id,
                                SizeId = size.Id,
                                IsActive = false,
                                Quantity = model.Quantity,
                                BusinessId = HttpContext.GetBusinessId(),
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = HttpContext.GetUserId(),
                                IsDeleted = false
                            };
                            ProductColorSize addedProductColorSize = await _unitOfWork.ProductColorSizes.AddAsync(newProductColorSize);

                            if (addedProductColorSize != null && await _unitOfWork.Complete())
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
                            _unitOfWork.ProductColorSizes.Delete(addedProductColorSize);
                            return BadRequest("Error in Add Size For Product !!");
                        }
                        return BadRequest("This Size Doesn't Exist !!");
                    }
                    return NotFound("This Product Color Doesn't Exist !!");
                }
                return BadRequest(ModelState);
            }
            return NotFound("This Product Color Doesn't Exist !!");
        }


        // api/ProductColorSize/{ProductColorId} => Delete ProductColorSize  
        [HttpDelete("{ProductColorSizeId}")]
        public async Task<IActionResult> DeleteProductColorSize([FromRoute] int? ProductColorSizeId)
        {
            if (ProductColorSizeId != null)
            {
                ProductColorSize queryProductColorSize = await _unitOfWork.ProductColorSizes.FindAsync(e => e.Id == ProductColorSizeId && e.BusinessId == HttpContext.GetBusinessId());
                if (queryProductColorSize != null)
                {
                    queryProductColorSize.IsDeleted = true;
                    await _unitOfWork.Complete();

                    return NoContent();
                }
                return NotFound("This Product Color Size Doesn't Exist !!");
            }
            return NotFound("This Product Color Size Doesn't Exist !!");
        }

        #endregion
    }
}
