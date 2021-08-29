using Microsoft.AspNetCore.Mvc;
using Mini_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.Controllers
{
    public class AdministrationController : Controller
    {
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        /*[HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {

        }*/
            
    }
}
