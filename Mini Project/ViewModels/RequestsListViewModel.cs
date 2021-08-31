using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mini_Project.Models;

namespace Mini_Project.ViewModels
{
    public class RequestsListViewModel
    {
        public IEnumerable<Request> Requests { get; set; }
        [Required]
        public string Comment { get; set; }
        public int Id { get; set; }
    }
}