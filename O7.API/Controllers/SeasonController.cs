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
    public class SeasonController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SeasonController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Season:

        // api/Season => get Seasons
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Season> Seasons = await _unitOfWork.Seasons.FindAllAsync(e => e.BusinessId == HttpContext.GetBusinessId());
            if (Seasons != null)
            {
                return Ok(_mapper.Map<IEnumerable<SeasonDto>>(Seasons));
            }
            return NoContent();
        }

        // api/Season/{SeasonId} => get Seasons
        [HttpGet("{SeasonId}")]
        public async Task<IActionResult> GetSeason([FromRoute] int? SeasonId)
        {
            if (SeasonId != null)
            {
                var querySeason = await _unitOfWork.Seasons.FindAsync(e => e.BusinessId == HttpContext.GetBusinessId() && e.Id == SeasonId);
                if (querySeason != null)
                {
                    return Ok(_mapper.Map<SeasonDto>(querySeason));
                }
                return NotFound();
            }
            return NotFound();
        }

        // api/Season => create ProductSeason:
        [HttpPost("")]
        public async Task<IActionResult> CreateSeason([FromBody] AddSeasonDto dto)
        {
            if (ModelState.IsValid)
            {
                Season isExist = await _unitOfWork.Seasons.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                if (isExist == null)
                {
                    Season newSeason = new Season
                    {
                        Name = dto.Name,
                        IsActive = true,
                        BusinessId = HttpContext.GetBusinessId(),
                        CreatedBy = HttpContext.GetUserId(),
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var result = await _unitOfWork.Seasons.AddAsync(newSeason);

                    if (result != null && await _unitOfWork.Complete())
                    {
                        return Ok(_mapper.Map<SeasonDto>(result));
                    }
                    return BadRequest("SomeThing Goes Wrong !!");

                }
                return BadRequest("This Season is Already Exist");
            }
            return BadRequest(ModelState);
        }

        // api/Season/{SeasonId} => Update Season:
        [HttpPut("{SeasonId}")]
        public async Task<IActionResult> UpdateSeason([FromRoute] int? SeasonId, [FromBody] UpdateSeasonDto dto)
        {
            if (SeasonId != null)
            {
                if (ModelState.IsValid)
                {
                    Season querySeason = await _unitOfWork.Seasons.FindAsync(e => e.Id == SeasonId && e.BusinessId == HttpContext.GetBusinessId());
                    if (querySeason != null)
                    {
                        if (querySeason.Name == dto.Name)
                        {
                            querySeason.Name = dto.Name;
                            querySeason.IsActive = dto.IsActive;

                            if (await _unitOfWork.Complete())
                            {
                                return Ok(_mapper.Map<SeasonDto>(querySeason));
                            }
                            return BadRequest("SomeThing Goes Wrong !!");
                        }
                        else
                        {
                            Season isExist = await _unitOfWork.Seasons.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                            if (isExist == null)
                            {
                                querySeason.Name = dto.Name;
                                querySeason.IsActive = dto.IsActive;

                                if (await _unitOfWork.Complete())
                                {
                                    return Ok(_mapper.Map<SeasonDto>(querySeason));
                                }
                                return BadRequest("SomeThing Goes Wrong !!");
                            }
                            return BadRequest("This Season is Already Exist");
                        }
                    }
                    return NotFound();
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }

        // api/Season/{SeasonId} => Delete Season:
        [HttpDelete("{SeasonId}")]
        public async Task<IActionResult> DeleteSeason([FromRoute] int? SeasonId)
        {
            if (SeasonId != null)
            {
                Season querySeason = await _unitOfWork.Seasons.FindAsync(e => e.Id == SeasonId && e.BusinessId == HttpContext.GetBusinessId());
                if (querySeason != null)
                {
                    querySeason.IsDeleted = true;
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
