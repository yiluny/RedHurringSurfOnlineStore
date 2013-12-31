using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using ProjectA.ViewModels;

namespace ProjectA.Controllers
{
    public class AuthenticateAttribute :AuthorizeAttribute
    {
        /// <summary>
        /// check if user has logged in
        /// </summary>
        /// <param name="filterContext"></param>


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            if (context.Session["LoginUser"] == null)
                context.Response.Redirect("~/SignIn/signIn");
        }


      

    }
}