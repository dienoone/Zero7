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
    public class StyleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StyleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Style:

        // api/Style => get Styles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Style> Styles = await _unitOfWork.Styles.FindAllAsync(e => e.BusinessId == HttpContext.GetBusinessId());
            if (Styles != null)
            {
                return Ok(_mapper.Map<IEnumerable<StyleDto>>(Styles));
            }
            return NoContent();
        }

        // api/Style/{StyleId} => get Styles
        [HttpGet("{StyleId}")]
        public async Task<IActionResult> GetStyle([FromRoute] int? StyleId)
        {
            if (StyleId != null)
            {
                var queryStyle = await _unitOfWork.Styles.FindAsync(e => e.BusinessId == HttpContext.GetBusinessId() && e.Id == StyleId);
                if (queryStyle != null)
                {
                    return Ok(_mapper.Map<StyleDto>(queryStyle));
                }
                return NotFound();
            }
            return NotFound();
        }

        // api/Style => create Style:
        [HttpPost("")]
        public async Task<IActionResult> CreateStyle([FromBody] AddStyleDto dto)
        {
            if (ModelState.IsValid)
            {
                Style isExist = await _unitOfWork.Styles.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                if (isExist == null)
                {
                    Style newStyle = new Style
                    {
                        Name = dto.Name,
                        Description = dto.Description,
                        IsActive = true,
                        BusinessId = HttpContext.GetBusinessId(),
                        CreatedBy = HttpContext.GetUserId(),
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var result = await _unitOfWork.Styles.AddAsync(newStyle);

                    if (result != null && await _unitOfWork.Complete())
                    {
                        return Ok(_mapper.Map<StyleDto>(result));
                    }
                    return BadRequest("SomeThing Goes Wrong !!");

                }
                return BadRequest("This Style is Already Exist");
            }
            return BadRequest(ModelState);
        }

        // api/Style/{StyleId} => Update Style:
        [HttpPut("{StyleId}")]
        public async Task<IActionResult> UpdateStyle([FromRoute] int? StyleId, [FromBody] UpdateStyleDto dto)
        {
            if (StyleId != null)
            {
                if (ModelState.IsValid)
                {
                    Style queryStyle = await _unitOfWork.Styles.FindAsync(e => e.Id == StyleId && e.BusinessId == HttpContext.GetBusinessId());
                    if (queryStyle != null)
                    {
                        if (queryStyle.Name == dto.Name)
                        {
                            queryStyle.Name = dto.Name;
                            queryStyle.Description = dto.Description;
                            queryStyle.IsActive = dto.IsActive;

                            if (await _unitOfWork.Complete())
                            {
                                return Ok(_mapper.Map<StyleDto>(queryStyle));
                            }
                            return BadRequest("SomeThing Goes Wrong !!");
                        }
                        else
                        {
                            Style isExist = await _unitOfWork.Styles.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                            if (isExist == null)
                            {
                                queryStyle.Name = dto.Name;
                                queryStyle.Description = dto.Description;
                                queryStyle.IsActive = dto.IsActive;

                                if (await _unitOfWork.Complete())
                                {
                                    return Ok(_mapper.Map<StyleDto>(queryStyle));
                                }
                                return BadRequest("SomeThing Goes Wrong !!");
                            }
                            return BadRequest("This Style is Already Exist");
                        }
                    }
                    return NotFound();
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }

        // api/Style/{StyleId} => Delete Style:
        [HttpDelete("{StyleId}")]
        public async Task<IActionResult> DeleteStyle([FromRoute] int? StyleId)
        {
            if (StyleId != null)
            {
                Style queryStyle = await _unitOfWork.Styles.FindAsync(e => e.Id == StyleId && e.BusinessId == HttpContext.GetBusinessId());
                if (queryStyle != null)
                {
                    queryStyle.IsDeleted = true;
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

