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
    public class GenderController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GenderController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Gender:

        // api/Gender => get Genders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Gender> Genders = await _unitOfWork.Genders.FindAllAsync(e => e.BusinessId == HttpContext.GetBusinessId());
            if (Genders != null)
            {
                return Ok(_mapper.Map<IEnumerable<GenderDto>>(Genders));
            }
            return NoContent();
        }

        // api/Gender/{GenderId} => get Genders
        [HttpGet("{GenderId}")]
        public async Task<IActionResult> GetGender([FromRoute] int? GenderId)
        {
            if (GenderId != null)
            {
                var queryGender = await _unitOfWork.Genders.FindAsync(e => e.BusinessId == HttpContext.GetBusinessId() && e.Id == GenderId);
                if (queryGender != null)
                {
                    return Ok(_mapper.Map<GenderDto>(queryGender));
                }
                return NotFound();
            }
            return NotFound();
        }

        // api/Gender => create ProductGender:
        [HttpPost("")]
        public async Task<IActionResult> CreateProductGender([FromBody] AddGenderDto dto)
        {
            if (ModelState.IsValid)
            {
                Gender isExist = await _unitOfWork.Genders.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                if (isExist == null)
                {
                    Gender newGender = new Gender
                    {
                        Name = dto.Name,
                        IsActive = true,
                        BusinessId = HttpContext.GetBusinessId(),
                        CreatedBy = HttpContext.GetUserId(),
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    var result = await _unitOfWork.Genders.AddAsync(newGender);

                    if (result != null && await _unitOfWork.Complete())
                    {
                        return Ok(_mapper.Map<GenderDto>(result));
                    }
                    return BadRequest("SomeThing Goes Wrong !!");

                }
                return BadRequest("This Gender is Already Exist");
            }
            return BadRequest(ModelState);
        }

        // api/Gender/{GenderId} => Update Gender:
        [HttpPut("{GenderId}")]
        public async Task<IActionResult> UpdateGender([FromRoute] int? GenderId, [FromBody] UpdateGenderDto dto)
        {
            if (GenderId != null)
            {
                if (ModelState.IsValid)
                {
                    Gender queryGender = await _unitOfWork.Genders.FindAsync(e => e.Id == GenderId && e.BusinessId == HttpContext.GetBusinessId());
                    if (queryGender != null)
                    {
                        if (queryGender.Name == dto.Name)
                        {
                            queryGender.Name = dto.Name;
                            queryGender.IsActive = dto.IsActive;

                            if (await _unitOfWork.Complete())
                            {
                                return Ok(_mapper.Map<GenderDto>(queryGender));
                            }
                            return BadRequest("SomeThing Goes Wrong !!");
                        }
                        else
                        {
                            Gender isExist = await _unitOfWork.Genders.FindAsync(e => e.Name == dto.Name && e.BusinessId == HttpContext.GetBusinessId());
                            if (isExist == null)
                            {
                                queryGender.Name = dto.Name;
                                queryGender.IsActive = dto.IsActive;

                                if (await _unitOfWork.Complete())
                                {
                                    return Ok(_mapper.Map<GenderDto>(queryGender));
                                }
                                return BadRequest("SomeThing Goes Wrong !!");
                            }
                            return BadRequest("This Gender is Already Exist");
                        }
                    }
                    return NotFound();
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }

        // api/Gender/{GenderId} => Delete Gender:
        [HttpDelete("{GenderId}")]
        public async Task<IActionResult> DeleteGender([FromRoute] int? GenderId)
        {
            if (GenderId != null)
            {
                Gender queryGender = await _unitOfWork.Genders.FindAsync(e => e.Id == GenderId && e.BusinessId == HttpContext.GetBusinessId());
                if (queryGender != null)
                {
                    queryGender.IsDeleted = true;
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
