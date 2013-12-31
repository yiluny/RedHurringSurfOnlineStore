using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectA.Models;

namespace ProjectA.ViewModels
{
    public class CombineViewModel
    {
        public ShoppingCartViewModel shoppingCart { get; set; }
        public signInViewModel signIn { get; set; }
     
    }
}