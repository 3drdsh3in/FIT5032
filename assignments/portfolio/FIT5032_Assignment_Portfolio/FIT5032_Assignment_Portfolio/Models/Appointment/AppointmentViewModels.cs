using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Web.UI.WebControls;
using System.Web.Mvc;

namespace FIT5032_Assignment_Portfolio.Models
{
    public class CreateAppointmentViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ChosenClientId")]
        public string ChosenClientEmail { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ClientsArray")]
        public List<ApplicationUser> ClientsArray { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Staff")]
        public string Staff { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointed Date Time Start")]
        public DateTime AppointedDateTimeStart { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointed Date Time End")]
        public DateTime AppointedDateTimeEnd { get; set; } // TODO: Validate On Controller that i.e smaller than AppointedDateTimeStart

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

    public class WrapperAppointmentViewModel
    {
        public ListAppointmentViewModel ListViewModel { get; set; }

        public DeleteAppointmentViewModel DeleteViewModel { get; set; }

        public List<SelectListItem> dropDownListItems { get; set; }
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
        [Display(Name = "ClientName")]
        public string ClientName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "StaffName")]
        public string StaffName { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointed Date Time Start")]
        public DateTime AppointedDateTimeStart { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointed Date Time End")]
        public DateTime AppointedDateTimeEnd { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Appointed Location Name")]
        public string AppointedLocationName { get; set; }

        [DataType(DataType.Text)]
        public string Description { get; set; }
    }

    public class UpdateAppointmentViewModel
    {
        public string Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointed Date Time Start")]
        public DateTime AppointedDateTimeStart { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointed Date Time End")]
        public DateTime AppointedDateTimeEnd { get; set; }

        [DataType(DataType.Text)]
        public string Description { get; set; }
    }

    public class DeleteAppointmentViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Id")]
        public string Id { get; set; }
    }

}