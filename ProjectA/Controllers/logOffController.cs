using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;

namespace ProjectA.Controllers
{
    public class logOffController : Controller
    {
        //
        // GET: /logOff/

        public ActionResult Index()
        {
            Session["LoginUser"] = null;

            if (HttpContext.Request.Cookies["User"] != null) 
            {
                var destroy = new HttpCookie("User");
                destroy.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(destroy);
            }


            return RedirectToAction("Index","Home");
        }

    }
}
