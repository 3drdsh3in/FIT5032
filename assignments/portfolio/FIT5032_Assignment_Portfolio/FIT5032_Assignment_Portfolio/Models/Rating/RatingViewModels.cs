using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace FIT5032_Assignment_Portfolio.Models
{
    public class CreateRatingViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "AppointmentId")]
        public string AppointmentId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "UserId")]
        public string UserId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "RatingValue")]
        public int RatingValue { get; set; }
    }

    public class GetAppointmentRatingsViewModel
    {
        // TODO
        public string AppointmentId { get; set; }
        public List<Rating> ratings { get; set; }
    }

    public class EditAppointmentRatingsViewModel
    {
        // TODO
    }
}