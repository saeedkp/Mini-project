using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mini_Project.Models;
using Mini_Project.Services;
using Mini_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mini_Project.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMailService mailService;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IRequestRepository _requestRepository;

        public AccountController(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                IMailService mailService,
                                IRequestRepository requestRepository,
                                RoleManager<IdentityRole> roleManager,
                                IWebHostEnvironment hostingEnvironment)
        {
            this.signInManager = signInManager;
            this.mailService = mailService;
            this.roleManager = roleManager;
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            _requestRepository = requestRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("createrequest", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    var roles = await userManager.GetRolesAsync(user);
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                        
                    }
                    else
                    {  
                        if(roles.Contains("HRM") || roles.Contains("Tech Lead")){
                            return RedirectToAction("requestslist", "home");
                        }
                        else
                        {
                            return RedirectToAction("CreateRequest", "home"); 
                        }
                          
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                Request request = _requestRepository.GetRequestByEmail(model.Email);

                string UniqueFileName = ProcessUploadedFile(model);

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    firstName = request.firstName,
                    lastName = request.lastName,
                };

                var result = await userManager.CreateAsync(user, model.Password);

                var roleResult = await userManager.AddToRoleAsync(user, "Trainee");

                if (result.Succeeded || roleResult.Succeeded)
                {
                    request.documentsPath = UniqueFileName;
                    request.state = State.WaitingForDocumentAcception;
                    _requestRepository.Update(request);
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("createrequest", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        private string ProcessUploadedFile(RegisterViewModel model)
        {
            string UniqueFileName = null;
            if (model.Documents != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "documents");
                UniqueFileName = Guid.NewGuid().ToString() + "_" + model.Documents.FileName;
                string filePath = Path.Combine(uploadsFolder, UniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Documents.CopyTo(fileStream);
                }
            }

            return UniqueFileName;
        }

    }
}
