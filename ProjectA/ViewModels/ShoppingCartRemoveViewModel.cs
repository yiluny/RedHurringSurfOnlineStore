using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectA.Models;

namespace ProjectA.ViewModels
{
    public class ShoppingCartRemoveViewModel
    {
        public string Message { get; set; }
        public double CartTotal { get; set; }
        public int CartCount { get; set; }
        public int ItemCount { get; set; }
        public int DeleteId { get; set; }
        public double ProductsubTotal{ get; set; } 
    }
}