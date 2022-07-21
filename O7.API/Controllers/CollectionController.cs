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
    public class CollectionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageHandler _imageHandler;
        private readonly AppURL _appURL;
        private readonly IWebHostEnvironment _environment;

        public CollectionController(IOptions<AppURL> AppURL, IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment environment, IImageHandler imageHandler)
        {
            _appURL = AppURL.Value;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _imageHandler = imageHandler;            
            _environment = environment;
        }

        // api/Collection
        [HttpGet]
        public async Task<IActionResult> GetCollections()
        {
            var entities = await _unitOfWork.Collections.GetCollectionsWithPhotos(HttpContext.GetBusinessId());
            if(entities != null)
            {
                return Ok(_mapper.Map<IEnumerable<CollectionDto>>(entities));
            }
            return NotFound();
        }

        // api/Collection/{collectionId}
        [HttpGet("{collectionId}")]
        public async Task<IActionResult> GetCollection([FromRoute] int? collectionId)
        {
            if(collectionId != null)
            {
                Collection collection = await _unitOfWork.Collections.GetCollectionWithPhotos(collectionId, HttpContext.GetBusinessId());
                if(collection != null)
                {
                    return Ok(_mapper.Map<CollectionDto>(collection));
                }
                return NotFound();
            }
            return NotFound();
        }

        // api/Collection
        [HttpPost]
        public async Task<IActionResult> CreateCollection([FromForm] AddCollectionDto model)
        {
            if (ModelState.IsValid)
            {
                Collection isExist = await _unitOfWork.Collections.FindAsync(e => e.Name == model.Name && e.BusinessId == HttpContext.GetBusinessId());
                if (isExist == null)
                {
                    // Check if Season is Exist:
                    Season isSeasonExist = await _unitOfWork.Seasons.FindAsync(e => e.Id == model.SeasonId);
                    if (isSeasonExist != null)
                    {
                        // Add new Collection:
                        Collection newCollection = new Collection
                        {
                            Name = model.Name,
                            IsActive = false,
                            SeasonId = model.SeasonId,
                            Description = model.Description,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = HttpContext.GetUserId(),
                            BusinessId = HttpContext.GetBusinessId()
                        };

                        Collection addedCollection = await _unitOfWork.Collections.AddAsync(newCollection);

                        // Chect if Collection Is Added :
                        if (addedCollection != null && await _unitOfWork.Complete())
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
                                            if (photo == null)
                                            {
                                                _unitOfWork.Collections.Delete(newCollection);
                                                return BadRequest(file);
                                            }
                                            else
                                            {
                                                CollectionPhoto newCollectionPhoto = new CollectionPhoto
                                                {
                                                    Photo = _appURL.AppUrl + "images/" + photo,
                                                    IsActive = false,
                                                    IsFavorite = false,
                                                    CollectionId = newCollection.Id,
                                                    IsDeleted = false,
                                                    CreatedAt = DateTime.UtcNow,
                                                    CreatedBy = HttpContext.GetUserId(),
                                                    BusinessId = HttpContext.GetBusinessId()
                                                };
                                                CollectionPhoto addedPhoto = await _unitOfWork.CollectionPhotos.AddAsync(newCollectionPhoto);
                                                if (addedPhoto == null)
                                                {
                                                    return BadRequest(file);
                                                }

                                            }
                                        }

                                    }
                                }

                                // ToDo: Return CollectionWith Photos:
                                if (await _unitOfWork.Complete())
                                {
                                    var result = await _unitOfWork.Collections.GetCollectionWithPhotos(addedCollection.Id, HttpContext.GetBusinessId());
                                    return Ok(_mapper.Map<CollectionDto>(result));
                                }
                                return BadRequest();
                            }
                        }
                        return BadRequest("Error in Create");
                    }
                    return BadRequest("Error in Create");
                }
                return BadRequest("This Name is Already Exist !!");
            }
            return BadRequest(ModelState);
        }

        // api/Collection/{collectionId}
        [HttpPut("{collectionId}")]
        public async Task<IActionResult> UpdateCollection([FromRoute] int? collectionId, [FromBody] UpdateCollectionDto model)
        {
            if (collectionId != null)
            {
                if (ModelState.IsValid)
                {
                    Collection queryCollection = await _unitOfWork.Collections.FindAsync(e => e.Id == collectionId && e.BusinessId == HttpContext.GetBusinessId());
                    if (queryCollection != null)
                    {
                        Season season = await _unitOfWork.Seasons.FindAsync(e => e.Id == model.SeasonId);
                        if (season != null)
                        {
                            queryCollection.Name = model.Name;
                            queryCollection.Description = model.Description;
                            queryCollection.IsActive = model.IsActive;
                            queryCollection.SeasonId = model.SeasonId;

                            // Update Exists Photos:
                            if (model.ExistPhotos != null)
                            {
                                foreach (var existPhoto in model.ExistPhotos)
                                {
                                    CollectionPhoto queryCollectionPhoto = await _unitOfWork.CollectionPhotos.FindAsync(e => e.Id == existPhoto.Id);
                                    if (queryCollectionPhoto != null)
                                    {
                                        queryCollectionPhoto.IsActive = existPhoto.IsActive;
                                        queryCollectionPhoto.IsFavorite = existPhoto.IsFavorite;
                                    }
                                }
                            }

                            // Delete Photos:
                            if (model.DeletedPhotos != null)
                            {
                                foreach (var detetedPhoto in model.DeletedPhotos)
                                {
                                    CollectionPhoto queryCollectionPhoto = await _unitOfWork.CollectionPhotos.FindAsync(e => e.Id == detetedPhoto.Id);
                                    if (queryCollectionPhoto != null)
                                    {
                                        var wwwroot = _environment.WebRootPath;
                                        var guid = queryCollectionPhoto.Photo.Remove(0, (_appURL.AppUrl.Length + 8) - 1).ToString();
                                        var path = Path.Combine(wwwroot, "images", guid);
                                        if (System.IO.File.Exists(path))
                                        {
                                            System.IO.File.Delete(path);
                                            _unitOfWork.CollectionPhotos.Delete(queryCollectionPhoto);
                                        }

                                    }

                                }

                            }

                            // ToDo: Return CollectionWith Photos:
                            if (await _unitOfWork.Complete())
                            {
                                var result = await _unitOfWork.Collections.GetCollectionWithPhotos(queryCollection.Id, HttpContext.GetBusinessId());
                                return Ok(_mapper.Map<CollectionDto>(result));
                            }
                            return BadRequest();
                        }
                        return BadRequest("This Season Does't Exist");
                    }
                    return NotFound();
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }

        // api/Collection/photos/{collectionId}
        [HttpPut("photos/{collectionId}")]
        public async Task<IActionResult> UpdateCollectionPhotos([FromRoute] int? collectionId, [FromForm] UpdateCollectionPhotoDto model)
        {
            if(collectionId != null)
            {
                Collection queryCollection = await _unitOfWork.Collections.FindAsync(e => e.Id == collectionId);
                if(queryCollection != null)
                {
                    //new Photos:
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
                                        // Create New Phtot:
                                        CollectionPhoto newCollectionPhoto = new CollectionPhoto
                                        {
                                            Photo = _appURL.AppUrl + photo,
                                            IsActive = false,
                                            IsFavorite = false,
                                            CollectionId = queryCollection.Id,
                                            IsDeleted = false,
                                            CreatedAt = DateTime.UtcNow,
                                            CreatedBy = HttpContext.GetUserId(),
                                            BusinessId = HttpContext.GetBusinessId()
                                        };
                                        // Check if Photo is Added:
                                        CollectionPhoto addedPhoto = await _unitOfWork.CollectionPhotos.AddAsync(newCollectionPhoto);

                                        if (addedPhoto == null)
                                        {
                                            return BadRequest(file);
                                        }
                                        await _unitOfWork.Complete();

                                    }
                                    else
                                    {
                                        return BadRequest(file);
                                    }

                                }

                            }

                        }

                    }

                    Collection collection = await _unitOfWork.Collections.GetCollectionWithPhotos(queryCollection.Id, HttpContext.GetBusinessId());
                    return Ok(_mapper.Map<CollectionDto>(collection));

                }
                return NotFound();
            }
            return NotFound();
        }

        // api/Collection/{collectionId}
        [HttpDelete("{collectionId}")]
        public async Task<IActionResult> DeleteCollection([FromRoute] int? collectionId)
        {
            if(collectionId != null)
            {
                Collection queryCollection = await _unitOfWork.Collections.FindAsync(e => e.Id == collectionId);
                if(queryCollection != null)
                {
                    queryCollection.IsDeleted = true;
                    await _unitOfWork.Complete();
                    return NoContent();
                }
                return NotFound();
            }
            return NotFound();
        }

    }
}
