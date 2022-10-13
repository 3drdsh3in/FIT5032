using FIT5032_Assignment_Portfolio.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace FIT5032_Assignment_Portfolio.Controllers
{
    public class SharedController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public SharedController()
        {
        }

        public SharedController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Shared
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult NavigationBar(string p)
        {
            //var entities = repository.GetEntities(p);
            var navigationBarViewModel = new NavigationBarViewModel();
            navigationBarViewModel.LoggedIn = false;
            navigationBarViewModel.ClientType = null;

            // Check If Logged In
            if (User.Identity.IsAuthenticated)
            {
                navigationBarViewModel.LoggedIn = true;

                var userIdentity = (ClaimsIdentity)User.Identity;
                var claims = userIdentity.Claims;
                var roleClaimType = userIdentity.RoleClaimType;

                // or...
                var roles = claims.Where(c => c.Type == roleClaimType).ToList();
               foreach (var role in roles)
                {
                    if (role.Value == "Client" || role.Value == "Staff") {
                        navigationBarViewModel.ClientType = role.Value; // Should only be one of the two for the current system.
                    }
                }
            }


            return PartialView(navigationBarViewModel);
        }
    }
}