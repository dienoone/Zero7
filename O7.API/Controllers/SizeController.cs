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
    public class SizeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SizeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Size:

        // api/size => get sizes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Size> sizes = await _unitOfWork.Sizes.FindAllAsync(e => e.BusinessId == HttpContext.GetBusinessId());
            if (sizes != null)
            {
                return Ok(_mapper.Map<IEnumerable<SizeDto>>(sizes));
            }
            return NoContent();
        }

        // api/size/{sizeId} => get Size
        [HttpGet("{sizeId}")]
        public async Task<IActionResult> GetSize([FromRoute] int? sizeId)
        {
            if (sizeId != null)
            {
                var querySize = await _unitOfWork.Sizes.FindAsync(e => e.BusinessId == HttpContext.GetBusinessId() && e.Id == sizeId);
                if (querySize != null)
                {
                    return Ok(_mapper.Map<SizeDto>(querySize));
                }
                return NotFound();
            }
            return NotFound();
        }

        // api/size => create size:
        [HttpPost()]
        public async Task<IActionResult> CreateSize([FromBody] AddSizeDto dto)
        {
            if (ModelState.IsValid)
            {
                Size isExist = await _unitOfWork.Sizes.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                if (isExist == null)
                {
                    Size newSize = new Size
                    {
                        Name = dto.Name,
                        Description = dto.Description,
                        IsActive = true,
                        BusinessId = HttpContext.GetBusinessId(),
                        CreatedBy = HttpContext.GetUserId(),
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var result = await _unitOfWork.Sizes.AddAsync(newSize);

                    if (result != null && await _unitOfWork.Complete())
                    {
                        return Ok(_mapper.Map<SizeDto>(result));
                    }
                    return BadRequest("SomeThing Goes Wrong !!");

                }
                return BadRequest("This Color is Already Exist");
            }
            return BadRequest(ModelState);
        }

        // api/Size/{sizeId} => Update Size:
        [HttpPut("{sizeId}")]
        public async Task<IActionResult> UpdateSize([FromRoute] int? sizeId, [FromBody] UpdateSizeDto dto)
        {
            if (sizeId != null)
            {
                if (ModelState.IsValid)
                {
                    Size querySize = await _unitOfWork.Sizes.FindAsync(e => e.Id == sizeId && e.BusinessId == HttpContext.GetBusinessId());
                    if (querySize != null)
                    {
                        if (querySize.Name == dto.Name)
                        {
                            querySize.Name = dto.Name;
                            querySize.Description = dto.Description;
                            querySize.IsActive = dto.IsActive;

                            if (await _unitOfWork.Complete())
                            {
                                return Ok(_mapper.Map<SizeDto>(querySize));
                            }
                            return BadRequest("SomeThing Goes Wrong !!");
                        }
                        else
                        {
                            Size isExist = await _unitOfWork.Sizes.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                            if (isExist == null)
                            {
                                querySize.Name = dto.Name;
                                querySize.Description = dto.Description;
                                querySize.IsActive = dto.IsActive;

                                if (await _unitOfWork.Complete())
                                {
                                    return Ok(_mapper.Map<SizeDto>(querySize));
                                }
                                return BadRequest("SomeThing Goes Wrong !!");
                            }
                            return BadRequest("This Size is Already Exist");
                        }
                    }
                    return NotFound();
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }

        // api/Size/{sizeId} => Delete Size:
        [HttpDelete("{sizeId}")]
        public async Task<IActionResult> DeleteSize([FromRoute] int? sizeId)
        {
            if (sizeId != null)
            {
                Size querySize = await _unitOfWork.Sizes.FindAsync(e => e.Id == sizeId && e.BusinessId == HttpContext.GetBusinessId());
                if (querySize != null)
                {
                    querySize.IsDeleted = true;
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
