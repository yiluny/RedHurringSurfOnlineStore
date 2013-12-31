using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;

namespace ProjectA.Controllers.backEnd
{
    public class StoresInformationController : Controller
    {
        ProjectDBContext db = new ProjectDBContext();
        //
        // GET: /StoresInformation/

        public ActionResult StoresInformation(int shopId)
        {
            shopInfo shop = new shopInfo();

            var shopInfor = (from u in db.shopInfo where u.shopId.Equals(shopId) select u).First();
            shop.shopName = shopInfor.shopName;
            shop.shopDescription = shopInfor.shopDescription;
            shop.shopOpenTime = shopInfor.shopOpenTime;
            shop.shopContactNum = shopInfor.shopContactNum;
            shop.shopAddress = shopInfor.shopAddress;

            return View(shop);
        }

        [HttpPost]
        public ActionResult StoresInformation(int shopId, FormCollection collection)
        {
            shopInfo shop = new shopInfo();
            if (ModelState.IsValid)
            {
                shopInfo currentshop = db.shopInfo.First(s => s.shopId.Equals(shopId));
                currentshop.shopDescription = collection["shopDescription"];
                currentshop.shopName = collection["shopName"];
                currentshop.shopOpenTime = collection["shopOpenTime"];
                currentshop.shopAddress = collection["shopAddress"];
                currentshop.shopContactNum = collection["shopContactNum"];
                db.SaveChanges();
            }
            shop.shopName = collection["shopName"];
            shop.shopDescription = collection["shopDescription"];
            shop.shopOpenTime = collection["shopOpenTime"];
            shop.shopContactNum = collection["shopContactNum"];
            shop.shopAddress = collection["shopAddress"];
            return View(shop);
        }
    }
}
