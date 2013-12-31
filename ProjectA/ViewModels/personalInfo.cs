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
    public class personalInfo
    {
        [Key]
        public int userId { get; set; }

        [Required]
        [DisplayName("UserName")]
        [RegularExpression(@"([\w!#]+)",
            ErrorMessage = "Only letters, number, '!'  '#',  '_' allowed")]
        [StringLength(12, MinimumLength = 5, ErrorMessage = "User name must be 5-12 characters")]
        //[Remote("doesUserNameExist", "Register", HttpMethod = "POST", ErrorMessage = "User Name already exists")]
        public String userName { get; set; }


        [Required]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [EmailAddress]
        public String email { get; set; }


        [Required]
        [DisplayName("First Name")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Fist name must be 1-30 characters")]
        public String firstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Last name must be 1-30 characters")]
        public String lastName { get; set; }


        /*-----------------Data belongs to Address table---------------------*/
        [Required]
        [DisplayName("State")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "State name must be 1-30 characters")]
        public String State { get; set; }

        [Required]
        [DisplayName("City")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "City name must be 1-30 characters")]
        public String City { get; set; }

        [Required]
        [DisplayName("Suburban")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "City name must be 1-50 characters")]
        public String Suburban { get; set; }


        [Required]
        [DisplayName("Post Code")]
        [RegularExpression(@"^[\S]+$", ErrorMessage = "Invalid postcode")]
        [postCodeValidation(ErrorMessage = "post length is too long")]
        //[StringLength(4, MinimumLength = 4, ErrorMessage = "Invalid postcode length")]
        //[DataType(DataType.PostalCode, ErrorMessage = "Invalid")]
        public int postcode { get; set; }


        [DisplayName("Moblie")]
        [RegularExpression(@"^[\S]+$", ErrorMessage = "Must be valid phone number")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Must be valid phone number")]
        public String phone { get; set; }


        [DisplayName("Address Line")]
        [StringLength(100, ErrorMessage = "City name must be 100 characters maximum")]
        public String addressLine1 { get; set; }


        [StringLength(100, ErrorMessage = "City name must be 100 characters maximum")]
        public String addressLine2 { get; set; }


        [DisplayName("Birthday")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid")]
        public DateTime? birthday { get; set; }

        /*---------------------------------------------*/


        /*[Key]
        public int userId { get; set; }

        [Required]
        [DisplayName("UserName")]
        //[RegularExpression(@"([\w!#]+)",
            //ErrorMessage = "Only letters, number, '!'  '#',  '_' allowed")]
        //[StringLength(12, MinimumLength = 5, ErrorMessage = "User name must be 5-12 characters")]
        //[Remote("doesUserNameExist", "Register", HttpMethod = "POST", ErrorMessage = "User Name already exists")]
        public String userName { get; set; }

        [Required]
        [DisplayName("Moblie")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Must be valid phone number")]
        public String phone { get; set; }

        [Required]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [EmailAddress]
        public String email { get; set; }

        [Required]
        [DisplayName("Birthday")]
        [DataType(DataType.Date)]
        public DateTime birthday { get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Fist name must be 1-30 characters")]
        public String firstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Last name must be 1-30 characters")]
        public String lastName { get; set; }*/


        /*-----------------Data belongs to Address table---------------------*/
        /*[Required]
        [DisplayName("State")]
        public String State { get; set; }

        [Required]
        [DisplayName("City")]
        public String City { get; set; }

        [Required]
        [DisplayName("Suburban/Town")]
        public String Suburban { get; set; }

        [Required]
        [DisplayName("Address Line")]
        public String addressLine { get; set; }

        [Required]
        [DisplayName("Post Code")]
        public int postcode { get; set; }*/
        /*---------------------------------------------*/


    }

    
}