using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectA.Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectA.ViewModels
{
    public class ShoppingCartViewModel
    {
        public string CartId { get; set; } 
        public List<Cart> CartItems { get; set; }
        public double CartTotal { get; set; }
        public double CartTax { get; set; }
        public double TotalAftertTax { get; set; }
    }
}