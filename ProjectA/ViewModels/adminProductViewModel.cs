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
using ProjectA.Models;

namespace ProjectA.ViewModels
{
    [Bind(Exclude = "productId")]
    public class adminProductViewModel
    {
        [Key]
        public int productId { get; set; }


        public string productName { get; set; }
        public double productPrice { get; set; }
        public string productDescription { get; set; }

        public Boolean? ifNewArrival { get; set; }
        public Boolean? ifTopSales { get; set; }
        public Boolean? ifPromoted { get; set; }
        public double? PromotedPrice { get; set; }
        public int? stockNumber { get; set; }

        public String MainCategoryName { get; set; }
        public String SubCategoryName { get; set; }

        public IList<Images> images { get; set; }
    }


}