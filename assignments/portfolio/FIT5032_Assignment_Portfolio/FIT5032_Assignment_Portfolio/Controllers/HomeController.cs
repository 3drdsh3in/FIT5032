using FIT5032_Assignment_Portfolio.Models;
using FIT5032_Assignment_Portfolio.Models.Email;
using FIT5032_Assignment_Portfolio.Utils;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIT5032_Assignment_Portfolio.Controllers
{
    [RequireHttps]
    [Authorize(Roles  = "Admin")]
    public class HomeController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private BusinessDbContext db = new BusinessDbContext();

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Send_Email()
        {
            SendEmailViewModel model = new SendEmailViewModel();
            model.StaffMembers = new List<ApplicationUser>();


            string adminId = User.Identity.GetUserId();
            Business business = db.Businesses.Where(b => b.AdminId == adminId).Take(1).ToList()[0];

            db.BusinessEmployees
                .Where(be => be.BusinessId == business.Id)
                .ForEach(be =>
                {
                    ApplicationUser user = UserManager.FindById(be.UserId);
                    model.StaffMembers.Add(user);
                });

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send_Email(SendEmailViewModel model, HttpPostedFileBase postedFile)
        {
            string adminId = User.Identity.GetUserId();
            Business business = db.Businesses.Where(b => b.AdminId == adminId).Take(1).ToList()[0];
            List<ApplicationUser> StaffMembers = new List<ApplicationUser>();

            if (ModelState.IsValid)
            {
                if (model.SendAllChecked)
                {
                    // Bulk Email

                    db.BusinessEmployees
                        .Where(be => be.BusinessId == business.Id)
                        .ForEach(be =>
                        {
                            ApplicationUser user = UserManager.FindById(be.UserId);
                            StaffMembers.Add(user);
                        });
                    foreach (var staffMember in StaffMembers)
                    {
                        String toEmail = staffMember.Email;
                        SendEmail(model, postedFile, toEmail);
                    }
                } else
                {
                    // Single Email
                    String toEmail = model.ToEmail;
                    SendEmail(model, postedFile, toEmail);
                }
            }

            model.StaffMembers = StaffMembers;

            return View(model);
        }

        private void SendEmail(SendEmailViewModel model, HttpPostedFileBase postedFile, String toEmail)
        {
            try
            {
                String subject = model.Subject;
                String contents = model.Contents;

                EmailSender es = new EmailSender();
                es.Send(toEmail, subject, contents, postedFile);

                ViewBag.Result = "Email has been send.";

                ModelState.Clear();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void ToggleOffBodyPaddingMargin()
        /* Use to remove the padding & margin css for the body tag */
        {
            ViewBag.BodyPaddingMargin = false;
        }

    }
}