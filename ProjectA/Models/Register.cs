using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;



namespace ProjectA.Models
{
    
    [Bind(Exclude = "userId")]
    public class Register
    {
        [Key]

        public long userId { get; set; }
        public String userName { get; set; }
        public String password { get; set; }
        public String firstName { get; set; }
        public String lastName { get; set; }
        public String email { get; set; }
        public String phone { get; set; }
        public DateTime? birthday { get; set; }



        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string suburban { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        
        public int postcode { get; set; }
        public bool? privilege { get; set; }

    }
}
