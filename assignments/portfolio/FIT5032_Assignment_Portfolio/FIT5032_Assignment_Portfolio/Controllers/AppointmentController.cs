using FIT5032_Assignment_Portfolio.Models;
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
    [Authorize(Roles = "Client,Staff")]
    [RequireHttps]
    public class AppointmentController : Controller
    {
        private AppointmentDbContext db = new AppointmentDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private EmailSender emailSender = new EmailSender();

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
        [Authorize(Roles = "Client,Staff")]
        public ActionResult Appointments()
        {
            String userId = this.User.Identity.GetUserId();
            List<String> userRoleArr = this.UserManager.GetRoles(userId).ToList();
            String userRole;
            List<TableAppointment> appointments = new List<TableAppointment>();
            String staffUserName = "";
            String clientUserName = "";

            // Retrieve all appointment IDs belonging to the user.
            List<String> userAppointmentIds = new List<String>();
            db.Appointments.Where(a => a.ClientUserId == userId || a.StaffUserId == userId).ForEach(a => {
                userAppointmentIds.Add(a.Id);
            });

            // Retrieve all appointments for the user and pass to the frontend.
            Appointment appointment = null;


            List<AppointmentUser> appointmentUsers = new List<AppointmentUser>();

            db.AppointmentUsers
                .Where(au => userAppointmentIds.Contains(au.AppointmentId))
                .ForEach(au => appointmentUsers.Add(au));

            TableAppointment tblAppointment = new TableAppointment();

            foreach (var id in userAppointmentIds)
            {
                appointment = (Appointment) db.Appointments.Where(a => a.Id == id).Take(1).ToList()[0];
                AppointmentUser clientAppointmentUser = (AppointmentUser) db.AppointmentUsers.Where(a => a.AppointmentId == id && a.UserRole == "Client").ToList()[0];
                AppointmentUser staffAppointmentUser = (AppointmentUser) db.AppointmentUsers.Where(a => a.AppointmentId == id && a.UserRole == "Staff").ToList()[0];

                if (appointment == null)
                {
                    throw new Exception("Error: No Appointment Found");
                }

                tblAppointment.Id = appointment.Id;
                tblAppointment.AppointedLocationName = appointment.AppointedLocationName;
                tblAppointment.AppointedDateTimeStart = appointment.AppointedDateTimeStart;
                tblAppointment.AppointedDateTimeEnd = appointment.AppointedDateTimeEnd;
                tblAppointment.Description = appointment.Description;

                tblAppointment.ClientName = UserManager.FindById(clientAppointmentUser.UserId).UserName;
                tblAppointment.StaffName = UserManager.FindById(staffAppointmentUser.UserId).UserName;

                appointments.Add(tblAppointment);
                tblAppointment = new TableAppointment();
            }

            ListAppointmentViewModel appointmentViewModel = new ListAppointmentViewModel();

            appointmentViewModel.appointments = appointments;

            WrapperAppointmentViewModel mymodel = new WrapperAppointmentViewModel();
            mymodel.ListViewModel = appointmentViewModel;
            mymodel.DeleteViewModel = new DeleteAppointmentViewModel();
            ViewBag.mymodel = mymodel;
            return View(mymodel); 
        }

        // GET: Create
        [HttpGet]
        [Authorize(Roles = "Staff")]
        public ActionResult Create()
        {
            CreateAppointmentViewModel createAppointmentModel = new CreateAppointmentViewModel();
            createAppointmentModel.ClientsArray = retrieveClientUsers();

            return View(createAppointmentModel);
        }

        private List<ApplicationUser> retrieveClientUsers()
        {
            List<ApplicationUser> relatedUsers = UserManager.Users.ToList();

            // OUT Array
            List<ApplicationUser> clientUsers = new List<ApplicationUser>();

            foreach (var user in relatedUsers)
            {
                List<string> roles = (List<string>)UserManager.GetRoles(user.Id);
                if (roles.Contains("Client"))
                {
                    clientUsers.Add(user);
                }
            }

            return clientUsers;
        }

        // Post: Create

        [HttpPost]
        [Authorize(Roles = "Staff")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateAppointmentViewModel model)
        {

            ApplicationUser client = UserManager.FindByEmail(model.ChosenClientEmail);
            ApplicationUser staff = UserManager.FindById(User.Identity.GetUserId());

            // Check if the created appointment overlaps with any other timeslot
            if (checkIfAppointmentOverlaps(model, staff.Id, client.Id))
            {
                ModelState.AddModelError("", "Appointment Overlaps With An Existing Appointment");
                model.ClientsArray = retrieveClientUsers();
                return View(model);
            }

            // Add New Appointment
            Appointment appointment = new Appointment();

            if (client == null)
            {
                ModelState.AddModelError("", "Corresponding Client was not found");
                model.ClientsArray = retrieveClientUsers();
                return View(model);
            }
            string AppointmentId = Guid.NewGuid().ToString("N");
            appointment.Id = AppointmentId;

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
            appointmentClientUser.UserId = client.Id;
            appointmentClientUser.UserRole = "Client";
            db.AppointmentUsers.Add(appointmentClientUser);


            appointment.ClientUserId = client.Id;
            appointment.StaffUserId = User.Identity.GetUserId();
            appointment.AppointedDateTimeStart = model.AppointedDateTimeStart;
            appointment.AppointedDateTimeEnd = model.AppointedDateTimeEnd;
            appointment.AppointedLocationLat = model.AppointedLocationLat;
            appointment.AppointedLocationLong = model.AppointedLocationLong;
            appointment.AppointedLocationName = model.AppointedLocationName;
            appointment.Description = model.Description;
            db.Appointments.Add(appointment);



            try
            {

                // Email Staff Notification
                ApplicationUser staffUser = UserManager.FindById(User.Identity.GetUserId());
                String staffUserEmail = staffUser.Email;
                String emailContentStaff = "You Have Booked A New Appointment With " + client.UserName;
                emailContentStaff += " From " + model.AppointedDateTimeStart + " To " + model.AppointedDateTimeEnd  + " At " + model.AppointedLocationName + ".";
                emailSender.Send(staffUserEmail, "You Have Booked New Appointment" , emailContentStaff, null);

                // Email Client Notification

                String clientUserEmail = model.ChosenClientEmail;
                String emailContentClient = "A New Appointment Has Been Booked With You By " + staffUser.UserName;
                emailContentClient += " From " + model.AppointedDateTimeStart + " To " + model.AppointedDateTimeEnd + " At " + model.AppointedLocationName + ".";
                emailSender.Send(clientUserEmail, "New Appointment", emailContentClient, null);
            } catch (Exception e) {
                throw e;
            }

            db.SaveChanges();
            return RedirectToAction("Appointments", "Appointment");
        }

        private bool checkIfAppointmentOverlaps(CreateAppointmentViewModel model, String staffId, String clientId)
        {
            bool appointmentOverlaps = false;

            // Loop through all the client & staff's existing appointments & Check if any of them overlap with the newly created appointment.
            List<Appointment> appointments = db.Appointments
            .Where(a => a.ClientUserId == clientId || a.StaffUserId == staffId)
            .ToList();

            appointments
            .ForEach(a =>
            {
                Console.WriteLine("AppointmentController");



                //bool startTimeLaterThanCurrentAppointmentStartTime = DateTime.Parse(model.AppointedDateTimeStart) > DateTime.Parse(a.AppointedDateTimeStart);
                //bool startTimeEarlierThanCurrentAppointmentEndTime = DateTime.Parse(model.AppointedDateTimeStart) < DateTime.Parse(a.AppointedDateTimeEnd);
                //bool endTimeLaterThanCurrentAppointmentStartTime = DateTime.Parse(model.AppointedDateTimeEnd) > DateTime.Parse(a.AppointedDateTimeStart);
                //bool endTimeEarlierThanCurrentAppointmentEndTime = DateTime.Parse(model.AppointedDateTimeEnd) < DateTime.Parse(a.AppointedDateTimeEnd);

                bool startTimeLaterThanCurrentAppointmentStartTime = model.AppointedDateTimeStart > a.AppointedDateTimeStart;
                bool startTimeEarlierThanCurrentAppointmentEndTime = model.AppointedDateTimeStart < a.AppointedDateTimeEnd;
                bool endTimeLaterThanCurrentAppointmentStartTime = model.AppointedDateTimeEnd > a.AppointedDateTimeStart;
                bool endTimeEarlierThanCurrentAppointmentEndTime = model.AppointedDateTimeEnd < a.AppointedDateTimeEnd;
                if (
                startTimeLaterThanCurrentAppointmentStartTime && startTimeEarlierThanCurrentAppointmentEndTime 
                ||
                endTimeLaterThanCurrentAppointmentStartTime && endTimeEarlierThanCurrentAppointmentEndTime
                ||
                !startTimeLaterThanCurrentAppointmentStartTime && !endTimeEarlierThanCurrentAppointmentEndTime
                )
                {
                    appointmentOverlaps = true;
                }
            });

            return appointmentOverlaps;
        }

        // Post: Delete
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("Delete/{Id}")]
        public ActionResult Delete(String Id)
        {

            Appointment deleteAppointment = db.Appointments.Find(Id);

            if (Id == null) return null;

            foreach (var appointmentUser in db.AppointmentUsers)
            {
                if (appointmentUser.AppointmentId == deleteAppointment.Id)
                {
                    db.AppointmentUsers.Remove(appointmentUser);
                }
            }

            db.Appointments.Remove(deleteAppointment);
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