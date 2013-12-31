using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Web.Mvc;

namespace ProjectA.Models
{
    public class SendEmail
    {
        ProjectDBContext dblg = new ProjectDBContext();
        [Required]
        [Display(Name = "EmailAddress")]
        public string EmailAddress { get; set; }

        public void ResetPasswordModel(string username)
        {
            var currentUser = (from u in dblg.register where u.userName.Equals(username) select u).FirstOrDefault();
            string password = currentUser.password;
            string emailname = currentUser.email;
            string userName = currentUser.userName;
            SendResetEmail(userName, emailname, password);
        }

        //Send Email Method
        public static void SendResetEmail(string userName, string emailname, string password)
        {
            MailMessage email = new MailMessage();

            email.From = new MailAddress("yilunyang987@gmail.com");
            email.To.Add(new MailAddress(emailname));

            email.Subject = "Password Reset";
            email.IsBodyHtml = true;
            string link = String.Format("<a href=\"http://localhost:20011/SignIn/signIn\">Click here to change your password.</a>", userName, HashResetParams(userName));
            email.Body = "<p>Hi " + userName + ",</p><br/>";
            email.Body += "<p>You recently asked to reset your RedHerring password.</p>";
            email.Body += "<p><a href='" + link + "'>" + link + "</a></p><br/>";
            email.Body += "<p>After logging in, please go to MyProfile --> Change Password to reset password</p><br/>";
            email.Body += "<p>We have set a new password for your account temporarily: </p><hr/><p>" + password + "</p><hr/>";
            email.Body += "<p>If you did not request a password reset you do not need to take any action.</p>";

            SmtpClient smtp = new SmtpClient();
            System.Net.NetworkCredential nc = new System.Net.NetworkCredential("yilunyang987@gmail.com", "987897798yyl");
            smtp.Credentials = nc;
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Timeout = 100000000;
            try
            {
                smtp.Send(email);
            }
            catch (Exception error) { }
        }

        //Method to hash parameters to generate the Reset URL
        public static string HashResetParams(string username)
        {

            byte[] bytesofLink = System.Text.Encoding.UTF8.GetBytes(username);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string HashParams = BitConverter.ToString(md5.ComputeHash(bytesofLink));

            return HashParams;
        }

    }
}