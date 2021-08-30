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
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IMailService mailService;
        private readonly IRequestRepository _requestRepository;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(IWebHostEnvironment hostingEnvironment,
                                IMailService mailService,
                                IRequestRepository requestRepository,
                                RoleManager<IdentityRole> roleManager,
                                UserManager<ApplicationUser> userManager)
        {
            _requestRepository = requestRepository;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            this.mailService = mailService;
        }

        public ViewResult index()
        {
            // if(User.IsInRole("HRM")){
            //     // return RedirectToAction("requestslist", "home");
            // }
            return View();
        }

        [HttpGet]
        public ViewResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(AddRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                string UniqueFileName = ProcessUploadedFile(model);

                Request newRequest = new Request
                {
                    firstName = model.firstName,
                    lastName = model.lastName,
                    Email = model.Email,
                    phoneNumber = model.phoneNumber,
                    resumePath = UniqueFileName
                };

                Random rnd = new Random();

                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = model.Email,
                    Subject = "Confirmation Email",
                    Body = "Your Code is : " + rnd.Next(1000, 10001),
                    Attachments = null
                };

                var result = SendMail(mailRequest);

                newRequest.state = State.FirstCheck;

                _requestRepository.Add(newRequest);

                var users = userManager.Users;
                foreach (var user in users)
                {
                    if (await userManager.IsInRoleAsync(user, "HRM"))
                    {
                        MailRequest mailRequestForHRM = new MailRequest
                        {
                            ToEmail = user.Email,
                            Subject = "New Request Available",
                            Body = "A New Request has been created," +
                        " Please check it in your panel. Thanks.",
                            Attachments = null
                        };

                        var resultHRM = SendMail(mailRequestForHRM);
                    }
                }

                return View("EmailSentConfirmation");

            }

            return View();
        }

        private string ProcessUploadedFile(AddRequestViewModel model)
        {
            string UniqueFileName = null;
            if (model.resume != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "resumes");
                UniqueFileName = Guid.NewGuid().ToString() + "_" + model.resume.FileName;
                string filePath = Path.Combine(uploadsFolder, UniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.resume.CopyTo(fileStream);
                }
            }

            return UniqueFileName;
        }

        private async Task<IActionResult> SendMail([FromForm] MailRequest request)
        {
            try
            {
                await mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public IActionResult RequestsList()
        {
            return View(_requestRepository.GetAllRequests());
        }
        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            string path = Path.Combine(hostingEnvironment.WebRootPath, "resumes/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/pdf", "resume.pdf");
        }
    }
}
