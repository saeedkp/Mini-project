﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mini_Project.Models
{
    public class Interview
    {
        public int Id { get; set; }
        [Required]
        [BindProperty, DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DateTime { get; set; }
        [Required]
        public bool Attendance { get; set; }
        [Required]
        public string Address { get; set; }
        public string Description { get; set; }
        [ForeignKey("Request")]
        public int RequestRefId { get; set; }

    }
}
