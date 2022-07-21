using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.ViewModels.O7ViewModels
{
    public class CollectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int SeasonId { get; set; }

        public List<CollectionPhotosDto> CollectionPhotos { get; set; }
    }

    public class AddCollectionDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int SeasonId { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
    public class UpdateCollectionDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int SeasonId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public List<CollectionPhotosDto> DeletedPhotos { get; set; }
        public List<CollectionPhotosDto> ExistPhotos { get; set; }

    }
    public class UpdateCollectionPhotoDto
    {
        public List<IFormFile> Photos { get; set; }
    }
    public class CollectionPhotosDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public bool IsFavorite { get; set; }
        [Required]
        public string Photo { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }

}
