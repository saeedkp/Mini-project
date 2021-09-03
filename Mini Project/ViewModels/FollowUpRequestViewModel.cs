using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.ViewModels
{
    public class FollowUpRequestViewModel
    {
        [Required]
        [Display(Name = "Follow Up Code")]
        public string code { get; set; }
    }
}
