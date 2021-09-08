using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.Models
{
    public class Request
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
            ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string phoneNumber { get; set; }

        [Required]
        public string resumePath { get; set; }

        public State? state { get; set; }
        public string followUpCode { get; set; }

        public string Comment { get; set; }

        public string documentsPath { get; set; }
    }
}
