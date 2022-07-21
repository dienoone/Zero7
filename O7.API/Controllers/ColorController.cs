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
    [Authorize(Roles ="Admin")]
    public class ColorController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ColorController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Color:

        // api/color => get Colors
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Color> colors = await _unitOfWork.Colors.FindAllAsync(e => e.BusinessId == HttpContext.GetBusinessId());
            if(colors != null)
            {
                return Ok(_mapper.Map<IEnumerable<ColorDto>>(colors));
            }
            return NoContent();
        }

        // api/color/{colorId} => get Colors
        [HttpGet("{colorId}")]
        public async Task<IActionResult> GetColor([FromRoute] int? colorId)
        {
            if(colorId != null)
            {
                var queryColor = await _unitOfWork.Colors.FindAsync(e => e.BusinessId == HttpContext.GetBusinessId() && e.Id == colorId);
                if(queryColor != null)
                {
                    return Ok(_mapper.Map<ColorDto>(queryColor));
                }
                return NotFound();
            }
            return NotFound();
        }

        // api/Color => create Color:
        [HttpPost("")]
        public async Task<IActionResult> CreateColor([FromBody] AddColorDto dto) 
        {
            if (ModelState.IsValid)
            {
                Color isExist = await _unitOfWork.Colors.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                if(isExist == null)
                {
                    Color newColor = new Color
                    {
                        Name = dto.Name,
                        Code = dto.Code,
                        IsActive = true,
                        BusinessId = HttpContext.GetBusinessId(),
                        CreatedBy = HttpContext.GetUserId(),
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var result = await _unitOfWork.Colors.AddAsync(newColor);

                    if(result != null && await _unitOfWork.Complete())
                    {
                        return Ok(_mapper.Map<ColorDto>(result));
                    }
                    return BadRequest("SomeThing Goes Wrong !!");

                }
                return BadRequest("This Color is Already Exist");
            }
            return BadRequest(ModelState);
        }

        // api/Color/{colorId} => Update Color:
        [HttpPut("{colorId}")]
        public async Task<IActionResult> UpdateColor([FromRoute]int? colorId, [FromBody] UpdateColorDto dto)
        {
            if(colorId != null)
            {
                if (ModelState.IsValid)
                {
                    Color queryColor = await _unitOfWork.Colors.FindAsync(e => e.Id == colorId && e.BusinessId == HttpContext.GetBusinessId());
                    if (queryColor != null)
                    {
                        if(queryColor.Name == dto.Name)
                        {
                            queryColor.Name = dto.Name;
                            queryColor.Code = dto.Code;
                            queryColor.IsActive = dto.IsActive;

                            if (await _unitOfWork.Complete())
                            {
                                return Ok(_mapper.Map<ColorDto>(queryColor));
                            }
                            return BadRequest("SomeThing Goes Wrong !!");
                        }
                        else
                        {
                            Color isExist = await _unitOfWork.Colors.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                            if (isExist == null)
                            {
                                queryColor.Name = dto.Name;
                                queryColor.Code = dto.Code;
                                queryColor.IsActive = dto.IsActive;

                                if (await _unitOfWork.Complete())
                                {
                                    return Ok(_mapper.Map<ColorDto>(queryColor));
                                }
                                return BadRequest("SomeThing Goes Wrong !!");
                            }
                            return BadRequest("This Color is Already Exist");
                        }                       
                    }
                    return NotFound();         
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }

        // api/Color/{colorId} => Delete Color:
        [HttpDelete("{colorId}")]
        public async Task<IActionResult> DeleteColor([FromRoute] int? colorId)
        {
            if(colorId != null)
            {
                Color queryColor = await _unitOfWork.Colors.FindAsync(e => e.Id == colorId && e.BusinessId == HttpContext.GetBusinessId());
                if(queryColor != null)
                {
                    queryColor.IsDeleted = true;
                    if(await _unitOfWork.Complete())
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
