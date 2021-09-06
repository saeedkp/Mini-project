using Mini_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.ViewModels
{
    public class AcceptedRequestListViewModel
    {
        public IEnumerable<Request> Requests { get; set; }
        [Required]
        public string Comment { get; set; }
        public int Id { get; set; }
    }
}
