using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using ProjectA.ViewModels;

namespace ProjectA.Controllers
{
    public class InformationController : Controller
    {
        ProjectDBContext db = new ProjectDBContext();



        //
        // GET: /Information/Weather

        public ActionResult Weather()
        {
            return View();
        }

        //
        //GET: /Information/Location



        public ActionResult Location()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetMessage(int locationId)
        {
            var results = new shopInfo();
            try
            {
                results = new shopInfo()
                {
                    shopId = locationId,
                    shopName = (from s in db.shopInfo where s.shopId.Equals(locationId) select s.shopName).First(),
                    shopDescription = (from s in db.shopInfo where s.shopId.Equals(locationId) select s.shopDescription).First(),
                    shopOpenTime = (from s in db.shopInfo where s.shopId.Equals(locationId) select s.shopOpenTime).First().Replace(Convert.ToString((char)13), "<br />"),
                    shopAddress = (from s in db.shopInfo where s.shopId.Equals(locationId) select s.shopAddress).First(),
                    shopContactNum = (from s in db.shopInfo where s.shopId.Equals(locationId) select s.shopContactNum).First(),
                };
            }
            catch (Exception error) 
            {

            }

            return Json(results);
        }

    }
}
