using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.ViewModels
{
    public class CorrectDocumentsViewModel
    {
        [Required]
        [Display(Name = "Documentations")]
        public IFormFile Documents { get; set; }
    }
}
