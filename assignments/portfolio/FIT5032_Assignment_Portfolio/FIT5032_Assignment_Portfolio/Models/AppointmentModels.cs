using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace FIT5032_Assignment_Portfolio.Models
{
    [Table("Appointments")]
    public partial class Appointment
    {
        public string Id { get; set; }
        public string ClientUserId { get; set; }

        public string AppointedDateTime { get; set; }

        public string AppointedLocationName { get; set; }

        public string AppointedLocationLat { get; set; }

        public string AppointedLocationLong { get; set; }

        public string Description { get; set; }
    }

    [Table("AppointmentUsers")]
    public partial class AppointmentUser
    {
        public string Id { get; set; }
        public string AppointmentId { get; set; }

        public string UserId { get; set; }

        public string UserRole { get; set; }
    }

    public class AppointmentDbContext : DbContext
    {
        public AppointmentDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Appointment> Appointments {get; set; }
        public DbSet<AppointmentUser> AppointmentsUsers { get; set; }

        public static AppointmentDbContext Create()
        {
            return new AppointmentDbContext();
        }
    }
}