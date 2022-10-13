using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace FIT5032_Assignment_Portfolio.Models
{
    public class NavigationBarViewModel
    {

        [DataType(DataType.Text)]
        public bool LoggedIn { get; set; }

        [DataType(DataType.Text)]
        public string ClientType { get; set; }
    }
}