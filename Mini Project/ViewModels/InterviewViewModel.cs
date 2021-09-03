using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mini_Project.Models;

namespace Mini_Project.ViewModels
{
    public class InterviewViewModel:Interview
    {
       public List<SelectListItem> Names {get;set;}
    }
}