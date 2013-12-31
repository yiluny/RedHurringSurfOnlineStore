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

namespace ProjectA.Controllers
{
    public class RegisterController : Controller
    {
        public ProjectDBContext db = new ProjectDBContext();


        //
        // GET: /Register/
        public ViewResult Index()
        {
            var states = (from s in db.Locations select s.state).Distinct().ToList();
            var cities = (from c in db.Locations where c.city == null select c.city).Distinct().ToList();

            ViewBag.States = new SelectList(states);
            ViewBag.Cities = new SelectList(cities);

            return View();
        }

        //
        // POST: /Register/
    
        /****Register as a member*****/
        [Recaptcha.RecaptchaControlMvc.CaptchaValidator]
        [HttpPost]
        public ActionResult Index(bool captchaValid, FormCollection collection, RegisterViewModel register)
        {
            var states = (from s in db.Locations select s.state).Distinct().ToList();

            ViewBag.States = new SelectList(states);

            if(!captchaValid)
            {
                ModelState.AddModelError("recaptcha_response_field", "invalid!");
                return View();
            }

            if (!register.TermsofUse)
            {
                ModelState.AddModelError("TermsOfUse", "please tick if you agree with terms of conditions");
                return View();
            }

            if (ModelState.IsValid && captchaValid && register.TermsofUse)
            {
                Register re = new Register();
                re.userName = collection["userName"];
                re.password = EncodePassword((string)collection["password"]);
                re.firstName = collection["firstName"];
                re.lastName = collection["lastName"];
                re.email = collection["email"];
                re.state = collection["state"];
                re.city = collection["city"];
                re.suburban = collection["suburban"];
                re.postcode = Convert.ToInt32(collection["postcode"]);
                re.country = "Australia";

                re.phone = collection["phone"];
                re.addressLine1 = collection["addressLine1"];
                re.addressLine2 = collection["addressLine2"];
                re.birthday = DateTime.ParseExact(collection["birthday"], "dd/mm/yyyy", CultureInfo.InvariantCulture);

                if (TryUpdateModel(re))
                {
                    db.register.Add(re);
                    re.password = EncodePassword((string)collection["password"]);
                    db.SaveChanges();

                    Session["LoginUser"] = collection["userName"];
                    return RedirectToAction("Index", "Home");
                }

            }
            return View(register);
        }





        /*-------------------------------
          Dynamic change of dropdown list
         ---------------------------------*/
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult change(string state)
        {
            var cities = from a in db.Locations where a.state == state select a.city;  
            var citiList = cities.ToList();
            var cityData = citiList.Select(a => new SelectListItem() { Value = a, Text = a });
            return Json(cityData, JsonRequestBehavior.AllowGet);
      
        }




        /*validate existing users*/
        [HttpPost]
        public JsonResult doesUserNameExist(String userName) 
        {
            var user = db.register.Where(u => u.userName.Equals(userName)).Select(u=>u.userName).ToList();
            return Json(user.Count==0);
        }


        [HttpPost]
        public JsonResult doesEmailExist(String Email)
        {
            var email = db.register.Where(u => u.email.Equals(Email)).Select(u => u.email).ToList();
            return Json(email.Count == 0);
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



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}