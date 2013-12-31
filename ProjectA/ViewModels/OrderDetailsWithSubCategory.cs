using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectA.Models;

namespace ProjectA.ViewModels
{
    public class OrderDetailsWithSubCategory
    {
        public OrderDetail orderdetail { get; set; }
        public string subcategoryName { get; set; }
        public string mainCategoryName { get; set; }
    }
}