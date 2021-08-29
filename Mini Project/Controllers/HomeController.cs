using Microsoft.AspNetCore.Hosting;
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
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IMailService mailService;
        private readonly IRequestRepository _requestRepository;

        public HomeController(IWebHostEnvironment hostingEnvironment,
                                IMailService mailService,
                                IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.mailService = mailService;
        }

        [HttpGet]
        public ViewResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRequest(AddRequestViewModel model)
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
                    Body = "Your Code is : " + rnd.Next(1000,10001),
                    Attachments = null
                };

                var result = SendMail(mailRequest);

                newRequest.state = State.FirstCheck;

                _requestRepository.Add(newRequest);

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

    }
}
