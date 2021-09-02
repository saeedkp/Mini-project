using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mini_Project.Models;

namespace Mini_Project.ViewModels
{
    public class InterviewsListViewModel
    {
        public IEnumerable<Interview> Interviews { get; set; }
        [Required]
        public string Comment { get; set; }
        public string Accept  { get; set; }
        public int requestId { get; set; }

    }
}