using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.ViewModels.O7ViewModels
{
    public class AddProudctColorSizeDto
    {
        [Required]
        public int SizeId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
    public class UpdateProudctColorSizeDto : AddProudctColorSizeDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public bool IsActive { get; set; }

    }
}
