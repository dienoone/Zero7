using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.ViewModels.O7ViewModels
{
    public class AddColorDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
    }
    public class UpdateColorDto : AddColorDto 
    { 
        [Required]
        public bool IsActive { get; set; }
    }
    public class ColorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }

}
