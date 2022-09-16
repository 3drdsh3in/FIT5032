using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace FIT5032_Assignment_Portfolio.Models
{
    public class CreateAppointmentViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Client")]
        public string Client { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Staff")]
        public string Staff { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointed Date Time")]
        public string AppointedDateTime { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Appointed Location Name")]
        public string AppointedLocationName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string AppointedLocationLat { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string AppointedLocationLong { get; set; }

        [DataType(DataType.Text)]
        public string Description { get; set; }
    }

    public class ListAppointmentViewModel
    {
        public List<TableAppointment> appointments { get; set; }
    }

    public class TableAppointment
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ClientId")]
        public string ClientId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "StaffId")]
        public string StaffId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointed Date Time")]
        public string AppointedDateTime { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Appointed Location Name")]
        public string AppointedLocationName { get; set; }

        [DataType(DataType.Text)]
        public string Description { get; set; }
    }
}