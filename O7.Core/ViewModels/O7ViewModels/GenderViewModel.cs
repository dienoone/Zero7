﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O7.Core.ViewModels.O7ViewModels
{
    public class GenderDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
    public class AddGenderDto
    {
        [Required]
        public string Name { get; set; }
    }

    public class UpdateGenderDto : AddGenderDto
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
