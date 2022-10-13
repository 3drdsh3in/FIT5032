using FIT5032_Assignment_Portfolio.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Dynamic;
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
        public ActionResult Appointments()
        {
            String userId = this.User.Identity.GetUserId();
            List<String> userRoleArr = this.UserManager.GetRoles(userId).ToList();
            String userRole;
            List<TableAppointment> appointments = new List<TableAppointment>();
            String preferredNameTitle = "Placeholder";


            // Retrieve all appointments for the user and pass to the frontend.
            Appointment appointment = null;

            foreach (AppointmentUser appointmentUser in db.AppointmentUsers.ToList())
            {
                TableAppointment tblAppointment = new TableAppointment();
                appointment = db.Appointments.Where(a => a.Id == appointmentUser.AppointmentId).Take(1).ToList()[0];
                if (appointmentUser.UserId == userId)
                {
                    switch (appointmentUser.UserRole)
                    {
                        case "Client":
                            tblAppointment.ClientId = appointmentUser.UserRole;
                            break;
                        case "Staff":
                            // Found Staff User. Search For Their Title and append to field.

                            // Look For StaffName (Should only be one):
                            List<ApplicationUser> user = UserManager.Users.Where(u => u.Id == appointmentUser.UserId).ToList();
                            if (user.Count == 1)
                            {
                                preferredNameTitle = user[0].PreferredNameTitle;
                            }
                                    
                            break;
                        default:
                            break;
                    }
                    if (appointment == null)
                    {
                        throw new Exception("Error: No Appointment Found");
                    }

                    tblAppointment.Id = appointment.Id;
                    tblAppointment.AppointedLocationName = appointment.AppointedLocationName;
                    tblAppointment.AppointedDateTimeStart = appointment.AppointedDateTimeStart;
                    tblAppointment.AppointedDateTimeEnd = appointment.AppointedDateTimeEnd;
                    tblAppointment.Description = appointment.Description;
                    tblAppointment.StaffName = preferredNameTitle;
                    tblAppointment.Rating = appointment.Rating;
                    tblAppointment.Complete = appointment.Complete;

                    appointments.Add(tblAppointment);
                }
            }

            ListAppointmentViewModel appointmentViewModel = new ListAppointmentViewModel();
            appointmentViewModel.appointments = appointments;

            WrapperAppointmentViewModel mymodel = new WrapperAppointmentViewModel();
            mymodel.ListViewModel = appointmentViewModel;
            mymodel.UpdateViewModel = new UpdateAppointmentViewModel();
            ViewBag.mymodel = mymodel;
            return View(mymodel); 
        }

        // GET: Create
        [HttpGet]
        public ActionResult Create()
        {
            CreateAppointmentViewModel createAppointmentModel = new CreateAppointmentViewModel();
            List<ApplicationUser> relatedUsers = UserManager.Users.ToList();

            // OUT Array
            List<ApplicationUser> clientUsers = new List<ApplicationUser>();

            foreach (var user in relatedUsers)
            {
                List<string> roles = (List<string>) UserManager.GetRoles(user.Id);
                if (roles.Contains("Client"))
                {
                    clientUsers.Add(user);
                }
            }

            createAppointmentModel.ClientsArray = clientUsers;

            return View(createAppointmentModel);
        }

        // Post: Create
        [HttpPost]
        public ActionResult Create(CreateAppointmentViewModel model)
        {
            // Add New Appointment
            Appointment appointment = new Appointment();
            ApplicationUser user = UserManager.FindByEmail(model.ChosenClientEmail);
            appointment.ClientUserId = user.Id;
            appointment.AppointedDateTimeStart = model.AppointedDateTimeStart;
            appointment.AppointedDateTimeEnd = model.AppointedDateTimeEnd;
            appointment.AppointedLocationLat = model.AppointedLocationLat;
            appointment.AppointedLocationLong = model.AppointedLocationLong;
            appointment.AppointedLocationName = model.AppointedLocationName;
            appointment.Description = model.Description;
            string AppointmentId = Guid.NewGuid().ToString("N");
            appointment.Id = AppointmentId;
            db.Appointments.Add(appointment);

            // Add New AppointmentUser (User Mapping to User)
            AppointmentUser appointmentStaffUser = new AppointmentUser();

            appointmentStaffUser.Id = Guid.NewGuid().ToString("N");
            appointmentStaffUser.AppointmentId = AppointmentId;
            appointmentStaffUser.UserId = User.Identity.GetUserId();
            appointmentStaffUser.UserRole = "Staff";
            db.AppointmentUsers.Add(appointmentStaffUser);

            AppointmentUser appointmentClientUser = new AppointmentUser();
            appointmentClientUser.Id = Guid.NewGuid().ToString("N");
            appointmentClientUser.AppointmentId = AppointmentId;
            appointmentClientUser.UserId = user.Id;
            appointmentClientUser.UserRole = "Client";
            db.AppointmentUsers.Add(appointmentClientUser);

            db.SaveChanges();
            return RedirectToAction("Appointments", "Appointment");
        }

        // Post: Delete
        [HttpPost]
        public ActionResult Delete(DeleteAppointmentViewModel model)
        {
            Appointment deleteAppointment = db.Appointments.Find(model.Id);

            foreach (var appointmentUser in db.AppointmentUsers)
            {
                if (appointmentUser.AppointmentId == deleteAppointment.Id)
                {
                    db.AppointmentUsers.Remove(appointmentUser);
                }
            }

            db.Appointments.Remove(deleteAppointment);
            db.SaveChanges();
            return RedirectToAction("Appointments","Appointment");
        }

        // Post: Update
        [HttpPost]
        public ActionResult UpdateAsClient(UpdateAppointmentViewModel model)
        {
            Appointment updateAppointment = db.Appointments.Find(model.Id);

            updateAppointment.Rating = model.Rating;
            
            db.Appointments.AddOrUpdate(updateAppointment);
            db.SaveChanges();
            return RedirectToAction("Appointments", "Appointment");
        }

        // Post: Update
        [HttpPost]
        public ActionResult UpdateAsStaff(WrapperAppointmentViewModel model)
        {
            UpdateAppointmentViewModel updateModel = model.UpdateViewModel;
            Appointment updateAppointment = db.Appointments.Find(updateModel.Id);

            if (updateModel.AppointedDateTimeStart != null && updateAppointment.AppointedDateTimeStart != updateModel.AppointedDateTimeStart) updateAppointment.AppointedDateTimeStart = updateModel.AppointedDateTimeStart;
            if (updateModel.AppointedDateTimeEnd != null && updateAppointment.AppointedDateTimeEnd != updateModel.AppointedDateTimeEnd) updateAppointment.AppointedDateTimeEnd = updateModel.AppointedDateTimeEnd;
            if (updateModel.Description != null && updateAppointment.Description != updateModel.Description) updateAppointment.Description = updateModel.Description;
            if (updateModel.Rating != 0 && updateModel.Rating <= 5 && updateAppointment.Rating != updateModel.Rating) updateAppointment.Rating = updateModel.Rating;
            if (updateAppointment.Complete != updateModel.Complete) updateAppointment.Complete = updateModel.Complete;

            db.Appointments.AddOrUpdate(updateAppointment);
            db.SaveChanges();
            return RedirectToAction("Appointments", "Appointment");
        }

        private void ToggleOffBodyPaddingMargin()
        /* Use to remove the padding & margin css for the body tag */
        {
            ViewBag.BodyPaddingMargin = false;
        }
    }

}