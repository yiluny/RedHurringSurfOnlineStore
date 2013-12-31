using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Web.Mvc;
namespace ProjectA.ViewModels
{
    public class AddNewProductViewModel
    {
        [Key]
        public int productId { get; set; }


        [Required]
        [StringLength(200, ErrorMessage = "ProductName must be less than 200 characters")]
        public string productName { get; set; }

        [Required]
        [DataType(DataType.Currency, ErrorMessage = "Must be valid value")]
        [RegularExpression(@"^[\S]+$", ErrorMessage = "No spaces allowed")]
        public double productPrice { get; set; }

        [StringLength(2000, ErrorMessage = "Description must be less than 2000 characters")]
        [AllowHtml]
        public string productDescription { get; set; }

        //[MaxLength]
        //public byte[] productImage { get; set; }
        [Required]
        public Boolean? ifNewArrival { get; set; }

        [Required]
        public Boolean? ifTopSales { get; set; }

        [Required]
        public Boolean? ifPromotion { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "Must be valid value")]
        [RegularExpression(@"^[\S]+$", ErrorMessage = "No spaces allowed")]
        public double? PromotedPrice { get; set; }


        //[RegularExpression(@"^\d+$", ErrorMessage = "Must be natural number")]
        [Required]
        [RegularExpression(@"^[\S]+$", ErrorMessage = "No spaces allowed")]
        //[NumberValidation]
        public int? stockNumber { get; set; }

        [Required]
        public String MainCategory { get; set; }

        public int SubCategoryId { get; set; }


        //[Required]
        public String SubCategory { get; set; }



        //public IList<Images> images { get; set; }

        //public IList<OrderDetail> orderDetails { get; set; }
    }

    public class NumberValidationAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string text = Convert.ToString(value);

            Regex regex = new Regex(@"^\d+$");

            if (!regex.IsMatch(text))
            {
                return new ValidationResult("Must be natural number");
            }

            //if (text.Length > 4)
            //{
            //    return new ValidationResult("");
            //}

            return ValidationResult.Success;
        }
    }
}