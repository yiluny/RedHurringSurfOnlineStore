using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Web.Security;
using ProjectA.ViewModels;

namespace ProjectA.Controllers
{
    public class HomeController : Controller
    {
        private ProjectDBContext db = new ProjectDBContext();

        public ActionResult Index()
        {
            if (Request.Browser.Browser == "IE" && float.Parse(Request.Browser.Version) < 8.0)
            {
                return View("ErrorIE7"); // Catches all IE6 users
            }

            var newArrival = (from p in db.Products select p).ToList();

            return View(newArrival);
        }



        public ActionResult GetImage(int id)
        {
            var newArrivalImage = (from p in db.image where p.productId == id && p.IfCoverPic == true select p).ToList();
            if (newArrivalImage.FirstOrDefault().Image != null)
            {
                var result = File(newArrivalImage.FirstOrDefault().Image, "image/jpeg");

                return result;
            }

            else
                return null;
        }




        public ActionResult GetImages(int id)
        {
            var newArrivalImage = (from p in db.image where p.productId == id select p).ToList();


            List<FileContentResult> img = new List<FileContentResult>();


            foreach (var i in newArrivalImage)
            {
                if (i != null)
                    img.Add(new FileContentResult(i.Image, "image/jpeg"));
            }

            ViewBag["images"] = img;

            return null;

        }

        public ActionResult About()
        {
            return View();
        }

        //
        //Get /Home/RedirectFromPaypal
        public ActionResult RedirectFromPaypal()
        {
            String txToken = Request.QueryString.Get("tx");
            String paymentStatus = Request.QueryString.Get("st");


            if (paymentStatus.Equals("Completed"))
            {
                var cartContext = ShoppingCart.GetCart(this.HttpContext);

                Guid orderId = cartContext.CreateOrder(new Order());
            }
            else
            {

            }
            return View();
        }

        //
        //Get Home/AdminLogIn
        public ActionResult AdminLogIn()
        {
            return View();
        }

        //post orders data to paypal
        //[HttpPost]
        public ActionResult PostToPayPal(String CartId, string shippingFee)
        {
            ProjectA.Models.Paypal paypal = new Models.Paypal();

            var cartContext = ShoppingCart.GetCart(this.HttpContext);

            var CartTax = cartContext.GetTax();
            var shipping = Convert.ToDouble(shippingFee);

            List<orderInformation> orders = new List<orderInformation>();

            var ProductsInCart = from cart in db.Carts where cart.CartId == CartId select cart;
            foreach (var product in ProductsInCart)
            {
                orders.Add(new orderInformation
                {
                    item_name = product.Product.productName,
                    quantity = product.Count,
                    unitPrice = product.Product.productPrice,
                    tax = Convert.ToDouble(string.Format("{0:0.00}", product.Product.productPrice / 10))
                });

            }
            paypal.orders = orders;


            paypal.cmd = "_cart";
            paypal.business = ConfigurationManager.AppSettings["BusinessAccountKey"];
            paypal.no_shipping = "0";

            paypal.shipping_1 = shipping;


            bool useSandbox = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSandbox"]);
            if (useSandbox)
                ViewData["actionURl"] = "https://www.sandbox.paypal.com/cgi-bin/webscr";
            else
                ViewData["actionURl"] = "https://www.paypal.com/cgi-bin/webscr";

            paypal.cancel_return = System.Configuration.ConfigurationManager.AppSettings["CancelURL"];
            paypal.@return = ConfigurationManager.AppSettings["ReturnURL"]; //+"&PaymentId=1"; you can append your order Id here
            paypal.notify_url = ConfigurationManager.AppSettings["NotifyURL"];// +"?PaymentId=1"; to maintain database logic 

            paypal.currency_code = ConfigurationManager.AppSettings["CurrencyCode"];


            return View(paypal);
        }



        [HttpPost]
        public ActionResult AdminLogIn(FormCollection collection)
        {
            var username = collection["Username"];
            var password = EncodePassword((string)collection["Password"]);
            bool Exists = db.register.Any(u => u.userName == username);

            if (String.IsNullOrEmpty(collection["Username"]) || String.IsNullOrEmpty(collection["Password"]))
            {
                if (String.IsNullOrEmpty(collection["Username"]))
                {
                    ModelState.AddModelError("Username", "User name is required");
                }

                if (String.IsNullOrEmpty(collection["Password"]))
                {
                    ModelState.AddModelError("Password", "Password is required");
                }
            }
            else
            {
                if (Exists)
                {
                    var passwordResult = (from r in db.register where r.userName.Equals(username) && r.privilege == true select r.password).First();
                    if ((!passwordResult.Equals(password)))
                    {
                        ModelState.AddModelError("AdminSignIn", "The username or password does not match!");

                    }
                    else
                    {
                        Session["Administrator"] = username;


                        return RedirectToAction("Index", "AdminHome");

                    }
                }
                else
                {
                    ModelState.AddModelError("AdminSignIn", "No such a administrator");
                }
            }
            return View();

        }


        /// <summary>
        /// In cryptography, MD5 (Message-Digest algorithm 5) is a widely used cryptographic hash function with a 128-bit hash value. 
        /// Specified in RFC 1321, MD5 has been employed in a wide variety of security applications, and is also commonly used to check the integrity of files. 
        /// However, it has been shown that MD5 is not collision resistant;[2] as such, MD5 is not suitable for applications like SSL certificates or digital signatures 
        /// that rely on this property. An MD5 hash is typically expressed as a 32-digit hexadecimal  number.
        /// </summary>
        /// <param name="originalPassword">original string</param>
        /// <returns></returns>
        public string EncodePassword(string originalPassword)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
            encodedBytes = md5.ComputeHash(originalBytes);

            // the following is if you want to returned the md5 hased password without dashes
            // return Regex.Replace(BitConverter.ToString(encodedBytes), "-", "");

            // Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }

    }
}
