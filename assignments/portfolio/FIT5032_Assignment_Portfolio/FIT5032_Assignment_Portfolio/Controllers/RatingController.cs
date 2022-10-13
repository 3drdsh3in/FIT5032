using FIT5032_Assignment_Portfolio.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIT5032_Assignment_Portfolio.Controllers
{
    public class RatingController : Controller
    {

        private AppointmentDbContext AppointmentDb = new AppointmentDbContext();
        private RatingDbContext ratingDb = new RatingDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public RatingController()
        {
        }

        public RatingController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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


        // GET: AppointmentRatings
        public ActionResult AppointmentRatings(GetAppointmentRatingsViewModel model)
        {
            String userId = User.Identity.GetUserId();
            List<Rating> userRatings = ratingDb.Ratings.Where(r => r.UserId == userId).ToList();
            return View(model);
        }
    }
}