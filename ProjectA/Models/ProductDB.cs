using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace ProjectA.Models
{
    
    public class ProductDB
    {
        [Key]
        public int productId { get; set; }

        public string productName { get; set; }

        public double productPrice { get; set; }

        
        public string productDescription { get; set; }

        public Boolean? ifNewArrival { get; set; }


        public Boolean? ifTopSales { get; set; }


        public Boolean? ifPromotion { get; set; }


        public double? PromotedPrice { get; set; }



        public int? stockNumber { get; set; }


        public String MainCategory { get; set; }

        public int SubCategoryId { get; set; }



        public virtual SubCategory SubCategory { get; set; }



        public IList<Images> images { get; set; }

        public IList<OrderDetail> orderDetails { get; set; }
        
    }

   

    public class NumberValidationAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string text = Convert.ToString(value);

            Regex regex = new Regex(@"^\d+$");

            if(!regex.IsMatch(text))
            {
                return new ValidationResult("Must be natural number");
            }

            return ValidationResult.Success;
        }
    }

   

}