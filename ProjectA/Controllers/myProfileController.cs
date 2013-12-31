using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.ViewModels;
using ProjectA.Models;
using System.Globalization;
using System.Web.UI;
using System.Security.Cryptography;
using System.Text;
using Webdiyer.WebControls.Mvc;
//using System.Threading;

namespace ProjectA.Controllers
{
    public class myProfileController : Controller
    {
        public ProjectDBContext db = new ProjectDBContext();

        //
        // GET: /myProfile/orderManagement?userName=Mike111
        public ActionResult orderManagement(string userName, int id = 1)
        {
            List<DateTime> datesWithoutTime = new List<DateTime>();
            var orderDates = (from r in db.Orders where r.userName == userName select r.OrderDate).Distinct();
            foreach (var orderDate in orderDates)
            {
                var date = orderDate.Date;
                datesWithoutTime.Add(date);
            }


            ViewBag.orders = (from r in db.Orders where r.userName == userName select r);


            PagedList<DateTime> distinctDates = datesWithoutTime.AsQueryable().OrderByDescending(o => o.Date).ToPagedList(id, 5);
            if (Request.IsAjaxRequest())
                return PartialView("orderManagementList", distinctDates);

            
            return View(distinctDates);
        }


        //
        // GET: /myProfile/OrderDetail?orderId=1
        public ActionResult OrderDetail(Guid orderId, int id=1)
        {

            PagedList<OrderDetail> OrderDetail = db.OrderDetails.Include("Product").Where(o => o.OrderId == orderId).OrderBy(o => o.Quantity).ToPagedList(id, 5);

            var order = db.Orders.Where(o => o.OrderId == orderId).FirstOrDefault();
            ViewBag.OrderId = orderId;
            ViewBag.Reasons = order.reasons;
            ViewBag.OrderStatus = order.OrderStatus;
            ViewBag.TotalPrice = order.totalPrice;

            return View(OrderDetail);
        }


        [HttpPost]
        public ActionResult getOrderReason(Guid orderId)
        {
            var order = db.Orders.Where(o => o.OrderId == orderId).FirstOrDefault();
            return Json(order.reasons.ToString());
        }


        //
        // GET: /myProfile/personalInfo
        public ActionResult personalInfo(String userName)
        {

            personalInfo personalInfo = new personalInfo();

            if (userName != null)
            {

                var userInfo = (from u in db.register where u.userName.Equals(userName) select u).First();


                personalInfo.userName = userInfo.userName;
                personalInfo.firstName = userInfo.firstName;
                personalInfo.lastName = userInfo.lastName;
                personalInfo.email = userInfo.email;
                personalInfo.State = userInfo.state;
                personalInfo.City = userInfo.city;
                personalInfo.Suburban = userInfo.suburban;
                personalInfo.postcode = userInfo.postcode;

                personalInfo.addressLine1 = userInfo.addressLine1;
                personalInfo.addressLine2 = userInfo.addressLine2;
                personalInfo.phone = userInfo.phone;           
                personalInfo.birthday = userInfo.birthday;
            }

            var states = (from s in db.Locations select s.state).Distinct().ToList();
            ViewBag.States = new SelectList(states);

            
            return View(personalInfo);
        }



        [HttpPost]
        public ActionResult personalInfo(FormCollection collection)
        {
            var states = (from s in db.Locations select s.state).Distinct().ToList();
            ViewBag.States = new SelectList(states);

            personalInfo personalInfo = new personalInfo();

            var user = (String)HttpContext.Session["LoginUser"];

            if(ModelState.IsValid && user!=null)
            {

                Register re = db.register.First(r => r.userName.Equals(user));
                re.firstName = collection["firstName"];
                re.lastName = collection["lastName"];
                re.email = collection["email"];
                re.state = collection["State"];
                re.city = collection["City"];
                re.suburban = collection["Suburban"];
                re.postcode = int.Parse(collection["postcode"], CultureInfo.InvariantCulture);

                re.addressLine1 = collection["addressLine1"];
                re.addressLine2 = collection["addressLine2"];
                re.phone = collection["phone"];
                re.birthday = DateTime.ParseExact(collection["birthday"], "dd/mm/yyyy", CultureInfo.InvariantCulture);
                


                db.SaveChanges();

                
                personalInfo.userName = re.userName;
                personalInfo.firstName = re.firstName;
                personalInfo.lastName = re.lastName;              
                personalInfo.email = re.email;
                personalInfo.State = re.state;
                personalInfo.City = re.city;
                personalInfo.Suburban = re.suburban;
                personalInfo.postcode = re.postcode;

                personalInfo.addressLine1 = re.addressLine1;
                personalInfo.addressLine2 = re.addressLine2;
                personalInfo.phone = re.phone;
                personalInfo.birthday = re.birthday;

                ViewBag.modifiedState = "success";
            }
            else
                ViewBag.modifiedState = "fail";

            return View(personalInfo);
        }
    



        //
        // GET: /myProfile/changePassword
        public ActionResult changePassword()
        {

            return View();
        }


        //
        // POST: /myProfile/changePassword
        [HttpPost]
        public ActionResult changePassword(FormCollection collection, String userName) 
        {
            var password = EncodePassword((string)collection["Oldpassword"]);

            var passwordValidation = db.register.Where(p => p.userName.Equals(userName)).First();

            if (password.Equals(passwordValidation.password) && ModelState.IsValid)
            {
                Register re = db.register.Where(r => r.userName.Equals(userName)).First();
                re.password = EncodePassword((string)collection["Newpassword"]);

                db.SaveChanges();

                ViewBag.modifiedState = "success";
            }
            else 
            {
                ModelState.AddModelError("passwordValidation","password does not match! ");
                ViewBag.modifiedState = "fail";
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
