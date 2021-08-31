using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Mini_Project.Models;

namespace Mini_Project.ViewModels
{
    public class InterviewViewModel
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
        public int RequestRefId { get; set; }
        
    }
}