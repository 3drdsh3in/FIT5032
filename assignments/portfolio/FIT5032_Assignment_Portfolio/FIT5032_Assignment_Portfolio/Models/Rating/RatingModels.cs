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
    [Table("Ratings")]
    public partial class Rating
    {
        public string Id { get; set; }
        public string AppointmentId { get; set; }
        public string UserId { get; set; }
        public int RatingValue { get; set; }

        
    }
    public class RatingDbContext : DbContext
    {
        public RatingDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Rating> Ratings { get; set; }

        public static AppointmentDbContext Create()
        {
            return new AppointmentDbContext();
        }
    }
}