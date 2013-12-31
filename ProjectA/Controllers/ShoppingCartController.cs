using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using ProjectA.ViewModels;

namespace ProjectA.Controllers
{
    public class ShoppingCartController : Controller
    {
        ProjectDBContext storeDB = new ProjectDBContext();
        List<Object> items = new List<Object>();
        //
        // GET: /ShoppingCart/

        public ActionResult Index(string CartId, string shippingFee)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartId = cart.GetCartId(this.HttpContext),
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal(),
                CartTax = cart.GetTax(),
                TotalAftertTax = cart.GetTax() + cart.GetTotal(),
            };

            var result = new CombineViewModel
            {
                shoppingCart = viewModel,
                signIn = new signInViewModel()
            };

            //Go to CheckOut if user want to
            if (CartId != null)
                return RedirectToAction("PostToPayPal", "Home", new { CartId = CartId, shippingFee = shippingFee });

            // Return the view
            return View(result);
        }

        //
        //POST: /ShoppingCart/
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            foreach (var recordId in collection)
            {
                //get the relavent cart Item
                var recordIdInt = Int32.Parse((string)recordId);
                if ((from cart in storeDB.Carts
                     where cart.RecordId == recordIdInt
                     select cart.CartId).Count() != 0)
                {
                    var cartItem = storeDB.Carts.Single(
                cart => cart.RecordId == recordIdInt);

                    //update cart Infor
                    cartItem.Count = Int32.Parse(collection[(string)recordId]);
                    cartItem.ProductSubTotal = Int32.Parse(collection[(string)recordId]) * cartItem.Product.productPrice;

                    //save changes
                    storeDB.SaveChanges();
                }
            }

            //back to shoppingCart Page
            return RedirectToAction("Index", "ShoppingCart");
        }

        //
        // GET: /ShoppingCart/CartSummary
        //[ChildActionOnly]
        public ActionResult smallCart()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            var viewModel = new ShoppingCartViewModel
            {
                CartId = cart.GetCartId(this.HttpContext),
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal(),
                CartTax = cart.GetTax(),
                TotalAftertTax = cart.GetTax() + cart.GetTotal(),
            };
            //var cart = ShoppingCart.GetCart(this.HttpContext);

            //ViewData["CartCount"] = cart.GetCount();
            //return PartialView("smallCart");
            return PartialView(viewModel);
        }

        //
        // GET: /Store/AddToCart/5
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity, string size)
        {
            // Retrieve the product from the database
            var addedProduct = storeDB.Products
                .Single(product => product.productId == id);

            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            cart.AddToCart(addedProduct, quantity, size);

            // Go back to the main store page for more shopping
            return RedirectToAction("Index", new { shopping = true });
        }

        [HttpPost]
        public ActionResult RemoveOne(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Get the name of the product to display confirmation
            string productName = storeDB.Carts
                .Single(item => item.RecordId == id).Product.productName;

            // Remove from cart
            int itemCount = cart.RemoveOne(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(productName) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ProductsubTotal = cart.GetProductSubTotal(id),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);

        }

        [HttpPost]
        public ActionResult AddOne(int id, string cartId)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Get the name of the product to display confirmation
            string productName = storeDB.Carts
                .Single(item => item.RecordId == id).Product.productName;

            // Add from cart
            int itemCount = cart.AddOne(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ProductsubTotal = cart.GetProductSubTotal(id),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }

        [HttpPost]
        public ActionResult DeleteOne(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            // Get the name of the product to display confirmation
            string productName = storeDB.Carts
                .Single(item => item.RecordId == id).Product.productName;

            // Remove from cart
            int itemCount = cart.DeleteOne(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = Server.HtmlEncode(productName) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }

        [HttpPost]
        public ActionResult TotalExclTax(int shippingFee)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext);
            var total = cart.GetTotal();
            var totalWithShippingFee = total + shippingFee;
            return Json(totalWithShippingFee);
        }

        //
        // GET: /ShoppingCart/CartSummary
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }


        //
        //GET:/ShoppingCart/EmptyAllCart

        public ActionResult EmptyAllCart()
        {
            var cart = new ShoppingCart();
            if (!String.IsNullOrWhiteSpace((String)HttpContext.Session["CartId"]))
            {

                var cartId = (String)HttpContext.Session["CartId"];

                cart.EmptyCart(cartId);
            }

            return RedirectToAction("Index", "ShoppingCart");
        }

    }
}
