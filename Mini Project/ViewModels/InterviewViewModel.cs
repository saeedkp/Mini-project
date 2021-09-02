using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Mini_Project.Models;

namespace Mini_Project.ViewModels
{
    public class InterviewViewModel:Interview
    {
       public Dictionary<string,string> Names {get;set;}
       [Required]
       public string Username { get; set; }
    }
}