using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace ProjectA.Models
{
    public class Location
    {
        [Key]
        public String country { get; set; }
        public String state { get; set; }
        public String city { get; set; }
    }
}