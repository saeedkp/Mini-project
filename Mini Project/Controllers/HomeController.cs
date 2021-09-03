using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mini_Project.Models;
using Mini_Project.Services;
using Mini_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
        private readonly IInterviewRepository _interviewRepository;

        public HomeController(IWebHostEnvironment hostingEnvironment,
                                IMailService mailService,
                                IRequestRepository requestRepository,
                                RoleManager<IdentityRole> roleManager,
                                UserManager<ApplicationUser> userManager,
                                IInterviewRepository interviewRepository)
        {
            _requestRepository = requestRepository;
            _interviewRepository = interviewRepository;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            this.mailService = mailService;
        }

        public ViewResult index()
        {
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

                string UniqueFollowUpCode = ProcessFollowUpCode(model);

                Request newRequest = new Request
                {
                    firstName = model.firstName,
                    lastName = model.lastName,
                    Email = model.Email,
                    phoneNumber = model.phoneNumber,
                    resumePath = UniqueFileName,
                    followUpCode = UniqueFollowUpCode
                };

                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = model.Email,
                    Subject = "Confirmation Email",
                    Body = "Your Code is : " + UniqueFollowUpCode,
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

        private string ProcessFollowUpCode(AddRequestViewModel model)
        {
            string UniqueCode = null;
            if (model.Email != null)
            {
                UniqueCode = Guid.NewGuid().ToString() + "-" + model.Email;
            }
            return UniqueCode;
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
        [HttpGet]
        public IActionResult RequestsList()
        {
            RequestsListViewModel requestsListViewModel = new RequestsListViewModel
            {
                Requests = _requestRepository.GetAllRequests(),
                Comment = ""
            };
            return View(requestsListViewModel);
        }
        [HttpPost]
        public IActionResult RequestsList(RequestsListViewModel model)
        {
            if (ModelState.IsValid)
            {
                Request request = _requestRepository.GetRequestById(model.Id);
                request.Comment = model.Comment;
                request.state = State.RejectByHRM;

                request = _requestRepository.Update(request);
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = request.Email,
                    Subject = "Internship Reject",
                    Body = request.Comment,
                    Attachments = null
                };

                var result = SendMail(mailRequest);
                return RedirectToAction("requestslist", "home");
            }
            return View();
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
        [HttpGet]
        public IActionResult Interview(int id, string type)
        {

            Interview newInterview = new Interview
            {
                DateTime = DateTime.Now,
                Address = "",
                Attendance = true,
                Description = "",
                RequestRefId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Type = type
            };
            return View(newInterview);
        }
        [HttpPost]
        public IActionResult Interview(Interview model)
        {
            if (ModelState.IsValid)
            {
                Interview newInterView = new Interview
                {
                    DateTime = model.DateTime,
                    Attendance = model.Attendance,
                    Address = model.Address,
                    Description = model.Description,
                    RequestRefId = model.RequestRefId,
                    UserId = model.UserId,
                    Type = model.Type
                };

                newInterView = _interviewRepository.Add(newInterView);
                Request request = _requestRepository.GetRequestById(model.RequestRefId);
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = request.Email,
                    Subject = "Accept for interview",
                    Body = "Interview Location: " + newInterView.Address + "<br />" +
                        "Interview Date and Time: " + newInterView.DateTime.ToString() + "<br />",

                    Attachments = null
                };

                var result = SendMail(mailRequest);
                request.state = State.InterviewWithHRM;
                _requestRepository.Update(request);

                return RedirectToAction("requestslist");
            }
            return View();
        }

        [HttpGet]
        public IActionResult InterviewsList()
        {
            InterviewsListViewModel interviewsListViewModel = new InterviewsListViewModel
            {
                Interviews = _interviewRepository.GetAllInterview().Where(interview => interview.Type == "first interview" &&
                interview.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value),
                Comment = ""

            };
            return View(interviewsListViewModel);
        }
        [HttpPost]
        public IActionResult InterviewsList(InterviewsListViewModel model)
        {
            if (ModelState.IsValid)
            {
                Request request = _requestRepository.GetRequestById(model.requestId);
                request.Comment = model.Comment;
                request.state = State.RejectAfterInterviewWithHRM;
                request = _requestRepository.Update(request);
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = request.Email,
                    Subject = "Internship Reject",
                    Body = request.Comment,
                    Attachments = null
                };
                var result = SendMail(mailRequest);

                return RedirectToAction("interviewslist", "home");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> SecondInterview(int id, string type)
        {
            // Dictionary<string, string> Employees = new Dictionary<string, string>();
            List<SelectListItem> items = new List<SelectListItem>();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var users = userManager.Users;
            foreach (var user in users)
            {
                if (await userManager.IsInRoleAsync(user, "HRM"))
                {
                    if (user.Id != userId)
                    {
                        string Name = user.firstName + " " + user.lastName;
                        items.Add(new SelectListItem {Text = Name, Value = user.Id});
                    }
                }
                if (await userManager.IsInRoleAsync(user, "TECH"))
                {
                    string Name = user.firstName + " " + user.lastName;
                    items.Add(new SelectListItem {Text = Name, Value = user.Id});
                }
            }
            InterviewViewModel interviewViewModel = new InterviewViewModel
            {
                DateTime = DateTime.Now,
                Address = "",
                Attendance = true,
                Description = "",
                RequestRefId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Type = type,
                Names = items
            };
            return View(interviewViewModel);
        }
        [HttpPost]
        public IActionResult SecondInterview(InterviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                    Interview newInterView = new Interview
                    {
                        DateTime = model.DateTime,
                        Attendance = model.Attendance,
                        Address = model.Address,
                        Description = model.Description,
                        RequestRefId = model.RequestRefId,
                        UserId = model.UserId,
                        Type = model.Type
                    };
                newInterView = _interviewRepository.Add(newInterView);
                Request request = _requestRepository.GetRequestById(model.RequestRefId);
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = request.Email,
                    Subject = "Second Interview",
                    Body = "We set second interview for you." + ".<br/>" +
                        "Interview Location: " + newInterView.Address + "<br />" +
                        "Interview Date and Time: " + newInterView.DateTime.ToString() + "<br />",

                    Attachments = null
                };

                var result = SendMail(mailRequest);
                request.state = State.InterviewWithHRM;
                _requestRepository.Update(request);

                return RedirectToAction("interviewslist");
            }
            return View();
        }

        [HttpGet]
        public IActionResult FollowRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FollowRequest(FollowUpRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                Request request = _requestRepository.GetRequestByFollowUpCode(model.code);

                if (Request == null)
                {
                    ViewBag.ErrorMessage = $"Request with Code = {model.code} cannot be found";
                    return View("NotFound");
                }
                return RedirectToAction("showstatus", "home", new {id = request.Id});
            }

            return View(model);
        }

        public IActionResult ShowStatus(int id)
        {
            Request request = _requestRepository.GetRequestById(id);
            return View(request);
        }
    }
}
