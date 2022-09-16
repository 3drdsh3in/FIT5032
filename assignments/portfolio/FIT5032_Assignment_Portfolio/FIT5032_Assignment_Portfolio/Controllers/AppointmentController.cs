using FIT5032_Assignment_Portfolio.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace FIT5032_Assignment_Portfolio.Controllers
{
    [Authorize(Roles = "Client,Staff")]
    public class AppointmentController : Controller
    {
        private AppointmentDbContext db = new AppointmentDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AppointmentController()
        {
        }

        public AppointmentController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: Appointment
        [HttpGet]
        public ActionResult Appointment()
        {
            String userId = this.User.Identity.GetUserId();
            List<TableAppointment> appointments = new List<TableAppointment>();
                 
            // Retrieve all appointments for the user and pass to the frontend.
            foreach (Appointment appointment in db.Appointments)
            {
                if (appointment.ClientUserId == userId)
                {
                    TableAppointment tblAppointment = new TableAppointment();

                    foreach (AppointmentUser appointmentUser in db.AppointmentsUsers)
                    {
                        if (appointmentUser.AppointmentId == appointment.Id)
                        {
                            switch (appointmentUser.UserRole)
                            {
                                case "Client":
                                    tblAppointment.ClientId = appointmentUser.UserRole;
                                    break;
                                case "Staff":
                                    tblAppointment.StaffId = appointmentUser.UserRole;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    tblAppointment.AppointedLocationName = appointment.AppointedLocationName;
                    tblAppointment.AppointedDateTime = appointment.AppointedDateTime;
                    tblAppointment.Description = appointment.Description;
                    appointments.Add(tblAppointment);
                }
            };

            ListAppointmentViewModel appointmentViewModel = new ListAppointmentViewModel();
            appointmentViewModel.appointments = appointments;

            return View(appointmentViewModel);
        }

        // GET: Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // Post: Create
        [HttpPost]
        public ActionResult Create(CreateAppointmentViewModel model)
        {
            Appointment appointment = new Appointment();
            appointment.ClientUserId = model.Client;
            appointment.AppointedDateTime = model.AppointedDateTime;
            appointment.AppointedLocationLat = model.AppointedLocationLat;
            appointment.AppointedLocationLong = model.AppointedLocationLong;
            appointment.AppointedLocationName = model.AppointedLocationName;
            db.Appointments.Add(appointment);
            return RedirectToAction("Appointment", "Appointment");
        }

        private void ToggleOffBodyPaddingMargin()
        /* Use to remove the padding & margin css for the body tag */
        {
            ViewBag.BodyPaddingMargin = false;
        }
    }

}