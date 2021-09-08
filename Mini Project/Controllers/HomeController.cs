using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
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
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IMailService mailService;
        private readonly IRequestRepository _requestRepository;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IInterviewRepository _interviewRepository;
        private readonly ILogger<HomeController> logger;

        public HomeController(IWebHostEnvironment hostingEnvironment,
                                IMailService mailService,
                                IRequestRepository requestRepository,
                                RoleManager<IdentityRole> roleManager,
                                UserManager<ApplicationUser> userManager,
                                IInterviewRepository interviewRepository,
                                ILogger<HomeController> logger)
        {
            _requestRepository = requestRepository;
            _interviewRepository = interviewRepository;
            this.logger = logger;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            this.mailService = mailService;
        }

        [AllowAnonymous]
        public ViewResult index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult CreateRequest()
        {
            return View();
        }
     
        [HttpPost]
        [AllowAnonymous]
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
                    Subject = "Follow Up Email",
                    Body = "You made a internship request." + "<br />" +
                    "Your follow up code to see the status of your " +
                    "request is : " + "<br />" + UniqueFollowUpCode,
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
                            Body = "A New internship request has been created." + "<br />" +
                            "Please check it in your panel in requests list. Thanks.",
                            Attachments = null
                        };

                        var resultHRM = SendMail(mailRequestForHRM);
                    }
                }

                return View("EmailSentConfirmation");

            }

            return View();
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
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
        [Authorize(Roles ="Admin, HRM")]
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
        [Authorize(Roles = "Admin, HRM")]
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

        [AllowAnonymous]
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
        [Authorize(Roles = "Admin, HRM")]
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
        [Authorize(Roles = "Admin, HRM")]
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
        [Authorize(Roles = "Admin, HRM")]
        public IActionResult InterviewsList()
        {
            IEnumerable<Interview> Interviews = _interviewRepository.GetAllInterview().Where(interview => interview.Type == "first interview" &&
                interview.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);
            foreach (var interview in Interviews)
            {
                if (interview.DateTime < DateTime.Now)
                {
                    Request request = _requestRepository.GetRequestById(interview.RequestRefId);
                    if (request.state != State.RejectAfterInterviewWithHRM && request.state != State.TechInterview)
                    {
                        request.state = State.EndInterviewWithHRM;
                    }

                }
            }

            List<Interview> interviewsList = _interviewRepository.GetAllInterview().Where(interview => interview.Type == "first interview" &&
                interview.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && interview.DateTime > DateTime.Now).ToList();

            interviewsList.Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
            var refreshTime = 0.0;
            if (interviewsList.Count != 0)
            {
                refreshTime = (interviewsList.First().DateTime - DateTime.Now).TotalSeconds;
            }

            InterviewsListViewModel interviewsListViewModel = new InterviewsListViewModel
            {
                Interviews = _interviewRepository.GetAllInterview().Where(interview => interview.Type == "first interview" &&
                interview.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value),
                Comment = "",
                RefreshTime = (int)refreshTime

            };
            return View(interviewsListViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, HRM")]
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
        [Authorize(Roles = "Admin, HRM, Tech Lead")]
        public IActionResult SecondInterviewsList()
        {
            IEnumerable<Interview> Interviews = _interviewRepository.GetAllInterview().Where(interview => interview.Type == "second interview" &&
                interview.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);
            foreach (var interview in Interviews)
            {
                if (interview.DateTime < DateTime.Now)
                {
                    Request request = _requestRepository.GetRequestById(interview.RequestRefId);
                    if (request.state != State.RejectAfterTechInterview && request.state != State.ObtainDocumentsAndOfficialProcess)
                    {
                        request.state = State.EndTechInterview;
                    }

                }
            }

            List<Interview> interviewsList = _interviewRepository.GetAllInterview().Where(interview => interview.Type == "second interview" &&
                interview.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && interview.DateTime > DateTime.Now).ToList();

            interviewsList.Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
            var refreshTime = 0.0;
            if (interviewsList.Count != 0)
            {
                refreshTime = (interviewsList.First().DateTime - DateTime.Now).TotalSeconds;
            }

            InterviewsListViewModel interviewsListViewModel = new InterviewsListViewModel
            {
                Interviews = _interviewRepository.GetAllInterview().Where(interview => interview.Type == "second interview" &&
                interview.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value),
                Comment = "",
                RefreshTime = (int)refreshTime

            };
            return View(interviewsListViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, HRM, Tech Lead")]
        public IActionResult SecondInterviewsList(InterviewsListViewModel model)
        {
            if (ModelState.IsValid)
            {

                Request request = _requestRepository.GetRequestById(model.requestId);
                request.Comment = model.Comment;
                if (model.Accept == "accept")
                {
                    request.state = State.ObtainDocumentsAndOfficialProcess;
                    request = _requestRepository.Update(request);
                    MailRequest mailRequest = new MailRequest
                    {
                        ToEmail = request.Email,
                        Subject = "Internship Accept",
                        Body = "Congratulations. You are accepted.",
                        Attachments = null
                    };
                    var result = SendMail(mailRequest);
                }
                else
                {
                    request.state = State.RejectAfterTechInterview;
                    request = _requestRepository.Update(request);
                    MailRequest mailRequest = new MailRequest
                    {
                        ToEmail = request.Email,
                        Subject = "Internship Reject",
                        Body = model.Comment,
                        Attachments = null
                    };
                    var result = SendMail(mailRequest);
                }



                return RedirectToAction("secondinterviewslist", "home");
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, HRM, Tech Lead")]
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
                        items.Add(new SelectListItem { Text = Name, Value = user.Id });
                    }
                }
                if (await userManager.IsInRoleAsync(user, "Tech Lead"))
                {
                    string Name = user.firstName + " " + user.lastName;
                    items.Add(new SelectListItem { Text = Name, Value = user.Id });
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
        [Authorize(Roles = "Admin, HRM, Tech Lead")]
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
                string Email = "";
                foreach (var user in userManager.Users)
                {
                    if (user.Id == model.UserId)
                    {
                        Email = user.Email;
                    }
                }
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
                MailRequest employeeMailRequest = new MailRequest
                {
                    ToEmail = request.Email,
                    Subject = "Second Interview",
                    Body = "We set second interview with you for " + request.firstName + " " + request.lastName
                     + ".<br/>" + "Interview Location: " + newInterView.Address + "<br />" +
                    "Interview Date and Time: " + newInterView.DateTime.ToString() + "<br />",

                    Attachments = null
                };

                var res = SendMail(employeeMailRequest);
                request.state = State.TechInterview;
                _requestRepository.Update(request);

                return RedirectToAction("interviewslist");
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult FollowRequest()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult FollowRequest(FollowUpRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                Request request = _requestRepository.GetRequestByFollowUpCode(model.code);

                if (request == null)
                {
                    ViewBag.ErrorMessage = $"Request with Code = {model.code} cannot be found";
                    return View("NotFound");
                }
                else
                {
                    return RedirectToAction("showstatus", "home", new { id = request.Id });
                }
                
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ShowStatus(int id)
        {
            Request request = _requestRepository.GetRequestById(id);
            return View(request);
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult IsEmailInUse(string email)
        {
            var request = _requestRepository.GetRequestByEmail(email);

            if (request == null)
            {
                return Json(true);   
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RequestExistsWithEmailAndIsAccepted(string email)
        {
            Request request = _requestRepository.GetRequestByEmail(email);

            if (request == null)
            {
                return Json($"There is no request with email : " +
                    $"{email}. Please make a request first.");
            }
            else if (request.state != State.ObtainDocumentsAndOfficialProcess)
            {
                return Json($"Your Request is in the state: " +
                    $" {request.state}. So you can't register " +
                    $"until you get accepted.");
            }
            else
            {
                return Json(true);
            }
            
        }

        [HttpGet]
        [Authorize(Roles = "Admin, HRM, Office Manager")]
        public IActionResult UserDocumentsList()
        {
            AcceptedRequestListViewModel acceptedRequestsListViewModel = new AcceptedRequestListViewModel
            {
                Requests = _requestRepository.GetAllRequests(),
                Comment = ""
            };
            return View(acceptedRequestsListViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, HRM, Office Manager")]
        public IActionResult UserDocumentsList(AcceptedRequestListViewModel model)
        {
            if (ModelState.IsValid)
            {
                Request request = _requestRepository.GetRequestById(model.Id);
                request.Comment = model.Comment;
                request.state = State.NeedDocumentsCorrection;

                request = _requestRepository.Update(request);
                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = request.Email,
                    Subject = "Need Document Correction",
                    Body = request.Comment,
                    Attachments = null
                };

                var result = SendMail(mailRequest);
                return RedirectToAction("userdocumentslist", "home");
            }
            return View();
        }

        [Authorize(Roles = "Admin, HRM, Office Manager")]
        public async Task<IActionResult> FullSignIn(int id)
        {
            if (ModelState.IsValid)
            {
                Request request = _requestRepository.GetRequestById(id);
                request.state = State.SuccessfulSignUp;

                var user = await userManager.FindByEmailAsync(request.Email);
                user.documentsPath = request.documentsPath;


                request = _requestRepository.Update(request);
                var resultUpdate = await userManager.UpdateAsync(user);

                MailRequest mailRequest = new MailRequest
                {
                    ToEmail = request.Email,
                    Subject = "Completed Sign Up",
                    Body = "Your sign up completed successfully",
                    Attachments = null
                };

                var result = SendMail(mailRequest);
                return RedirectToAction("userdocumentslist", "home");
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Trainee")]
        public IActionResult CorrectDocuments()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Trainee")]
        public async Task<IActionResult> CorrectDocuments(CorrectDocumentsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await userManager.FindByIdAsync(id);
                var request = _requestRepository.GetRequestByEmail(user.Email);

                string UniqueFileName = ProcessUploadedDoc(model);

                request.documentsPath = UniqueFileName;
                request.state = State.WaitingForDocumentAcception;
                user.documentsPath = UniqueFileName;

                _requestRepository.Update(request);
                var result = userManager.UpdateAsync(user);
;
                return RedirectToAction("index", "home");
            }

            return View(model);
        }


        [AllowAnonymous]
        public FileResult DownloadDocs(string fileName)
        {
            //Build the File Path.
            string path = Path.Combine(hostingEnvironment.WebRootPath, "documents/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/pdf", "document.pdf");
        }

        [AllowAnonymous]
        private string ProcessUploadedDoc(CorrectDocumentsViewModel model)
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
