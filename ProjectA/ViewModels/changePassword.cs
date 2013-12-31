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



namespace ProjectA.ViewModels
{

    [Bind(Exclude = "userId")]
    public class changePassword
    {
        [Key]
        public int userId { get; set; }

        [Required]
        [DisplayName("Old Password")]
        [DataType(DataType.Password)]
        public String Oldpassword { get; set; }

        [Required]
        [DisplayName("New Password")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password length must be 8-20")]
        [RegularExpression(@"([\S]+)", ErrorMessage = "White spaces is not allowed!")]
        [DataType(DataType.Password)]
        public String Newpassword { get; set; }

        [Required]
        [DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("Newpassword", ErrorMessage = "Password should be identical !")]
        public String confirmPassword { get; set; }
    }
}