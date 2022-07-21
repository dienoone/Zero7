using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using O7.API.Extentions;
using O7.Core;
using O7.Core.Models.O7Models.Main;
using O7.Core.ViewModels.O7ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace O7.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TypeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TypeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Type:

        // api/Type => get Types
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<ProductType> Types = await _unitOfWork.ProductTypes.FindAllAsync(e => e.BusinessId == HttpContext.GetBusinessId());
            if (Types != null)
            {
                return Ok(_mapper.Map<IEnumerable<TypeDto>>(Types));
            }
            return NoContent();
        }

        // api/Type/{TypeId} => get Types
        [HttpGet("{TypeId}")]
        public async Task<IActionResult> GetType([FromRoute] int? TypeId)
        {
            if (TypeId != null)
            {
                var queryType = await _unitOfWork.ProductTypes.FindAsync(e => e.BusinessId == HttpContext.GetBusinessId() && e.Id == TypeId);
                if (queryType != null)
                {
                    return Ok(_mapper.Map<TypeDto>(queryType));
                }
                return NotFound();
            }
            return NotFound();
        }

        // api/Type => create ProductType:
        [HttpPost("")]
        public async Task<IActionResult> CreateProductType([FromBody] AddTypeDto dto)
        {
            if (ModelState.IsValid)
            {
                ProductType isExist = await _unitOfWork.ProductTypes.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                if (isExist == null)
                {
                    ProductType newType = new ProductType
                    {
                        Name = dto.Name,
                        IsActive = true,
                        BusinessId = HttpContext.GetBusinessId(),
                        CreatedBy = HttpContext.GetUserId(),
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var result = await _unitOfWork.ProductTypes.AddAsync(newType);

                    if (result != null && await _unitOfWork.Complete())
                    {
                        return Ok(_mapper.Map<TypeDto>(result));
                    }
                    return BadRequest("SomeThing Goes Wrong !!");

                }
                return BadRequest("This Type is Already Exist");
            }
            return BadRequest(ModelState);
        }

        // api/Type/{ProductTypeId} => Update ProductType:
        [HttpPut("{ProductTypeId}")]
        public async Task<IActionResult> UpdateProductType([FromRoute] int? ProductTypeId, [FromBody] UpdateTypeDto dto)
        {
            if (ProductTypeId != null)
            {
                if (ModelState.IsValid)
                {
                    ProductType queryType = await _unitOfWork.ProductTypes.FindAsync(e => e.Id == ProductTypeId && e.BusinessId == HttpContext.GetBusinessId());
                    if (queryType != null)
                    {
                        if (queryType.Name == dto.Name)
                        {
                            queryType.Name = dto.Name;
                            queryType.IsActive = dto.IsActive;

                            if (await _unitOfWork.Complete())
                            {
                                return Ok(_mapper.Map<TypeDto>(queryType));
                            }
                            return BadRequest("SomeThing Goes Wrong !!");
                        }
                        else
                        {
                            ProductType isExist = await _unitOfWork.ProductTypes.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                            if (isExist == null)
                            {
                                queryType.Name = dto.Name;
                                queryType.IsActive = dto.IsActive;

                                if (await _unitOfWork.Complete())
                                {
                                    return Ok(_mapper.Map<TypeDto>(queryType));
                                }
                                return BadRequest("SomeThing Goes Wrong !!");
                            }
                            return BadRequest("This Type is Already Exist");
                        }
                    }
                    return NotFound();
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }

        // api/Type/{ProductTypeId} => Delete ProductType:
        [HttpDelete("{ProductTypeId}")]
        public async Task<IActionResult> DeleteProductType([FromRoute] int? ProductTypeId)
        {
            if (ProductTypeId != null)
            {
                ProductType queryType = await _unitOfWork.ProductTypes.FindAsync(e => e.Id == ProductTypeId && e.BusinessId == HttpContext.GetBusinessId());
                if (queryType != null)
                {
                    queryType.IsDeleted = true;
                    if (await _unitOfWork.Complete())
                    {
                        return NoContent();
                    }
                    return BadRequest("SomeThing Goes Wrong !!");
                }
                return NotFound();
            }
            return NotFound();
        }

        #endregion
    }
}
