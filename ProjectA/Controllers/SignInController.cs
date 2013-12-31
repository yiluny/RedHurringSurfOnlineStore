using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using ProjectA.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Web.Security;

namespace ProjectA.Controllers
{
    public class SignInController : Controller
    {
        public ProjectDBContext dblg = new ProjectDBContext();


        //
        // GET: /SignIn/signIn
        public ActionResult signIn()
        {
            return View();

        
        }

        [HttpPost]
        public ActionResult signIn(FormCollection collection)
        {
            var username = collection["Username"];
            var password = EncodePassword((string)collection["Password"]);
            bool Exists = dblg.register.Any(u => u.userName == username);


            if (String.IsNullOrEmpty(collection["Username"]))
            {
                ModelState.AddModelError("Username","User name is required");
            }

            if (Exists)
            {
                var passwordResult = (from r in dblg.register where r.userName.Equals(username) select r.password).First();
                if ((!passwordResult.Equals(password)))
                {
                    ModelState.AddModelError("SignIn", "The username or password does not match!");

                }
                else
                {
                    if (!collection["rememberMe"].Equals("false"))
                    {
                        HttpCookie cookie = new HttpCookie("User");
                        cookie.Values.Add("user", username.ToString());
                        cookie.Expires = DateTime.Now.AddDays(2);
                        Response.AppendCookie(cookie);
                    }

                    Session["LoginUser"] = username;

                   
                    return RedirectToAction("Index", "Home");
                    
                }
            }
            else
            {
                ModelState.AddModelError("SignIn", "No such a user");
            }
            return View();

       }


        

        
        //public ActionResult signIn(signInViewModel signIn)
        //[Recaptcha.RecaptchaControlMvc.CaptchaValidator]
        [HttpPost]
        public ActionResult checkOnshopCart(String Username, String password, String rememberMe)
        {
            if (String.IsNullOrWhiteSpace(Username) || String.IsNullOrWhiteSpace(password))
                return Json("User or password name is required");
            


            password = EncodePassword(password);
            bool Exists = dblg.register.Any(u => u.userName == Username);

            if (Exists)
            {
                var passwordResult = (from r in dblg.register where r.userName.Equals(Username) select r.password).First();
                if ((!passwordResult.Equals(password)))
                {
                    return Json("the username or password does not match!");
                }
                else
                {
                    if(Convert.ToBoolean(rememberMe))
                    {
                        HttpCookie cookie = new HttpCookie("User");
                        cookie.Values.Add("user", Username);
                        cookie.Expires = DateTime.Now.AddMinutes(15.0);
                        Response.AppendCookie(cookie);
                    }

                    Session["LoginUser"] = Username;

                    return RedirectToAction("Index", "ShoppingCart");
                }
            }
            else
            {
                ModelState.AddModelError("SignIn", "No such a user");
                return Json("No such a user");
                
            }

        }



        [HttpPost]
        public ActionResult Check()
        {
            return Json( (Session["LoginUser"] == null && HttpContext.Request.Cookies["User"]==null) );
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








        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }



        //Controllers for the reset page
        public ActionResult ResetPassword()
        {
            return View();
        }




        [HttpPost]
        public ActionResult ResetPassword(SendEmail Model)
        {
            var currentUser = (from u in dblg.register where u.email.Equals(Model.EmailAddress) select u).FirstOrDefault();

            if (currentUser != null)
            {
                //grab a new password and change the database
                string newPass = RandomString(8);
                currentUser.password = EncodePassword(newPass);
                dblg.SaveChanges();

                if (ModelState.IsValid)
                {
                    SendEmail.SendResetEmail(currentUser.userName, currentUser.email, newPass);
                }
                return RedirectToAction("SentEmail", "SignIn");
            }
            else
            {
                ModelState.AddModelError("ResetPassword", "We cannot find the e-mail address you entered.Please enter the correct e-mail address.");
                return View();
            }

        }

        public ActionResult SentEmail()
        {
            return View();
        }

    }
}

