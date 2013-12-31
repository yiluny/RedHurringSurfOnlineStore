using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using ProjectA.Models;

namespace ProjectA.Models
{

    public partial class SubCategory
    {
        public int SubCategoryId { get; set; }

        [Required]
        public string SubcategoryName { get; set; }

        public string MainCategoryName { get; set; }

        public List<ProductDB> Products { get; set; }


    }
}