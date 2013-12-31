using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using Webdiyer.WebControls.Mvc;

namespace ProjectA.Controllers
{
    public class PromotionController : Controller
    {
        //
        // GET: /Promotion/


        public ActionResult Index(int itemsPerPage = 6, int id = 1)
        {
            using (var db = new ProjectDBContext())
            {
                PagedList<ProductDB> promotedProducts = db.Products.Where(p => p.ifPromotion == true).OrderBy(o => o.productId).ToPagedList(id, itemsPerPage);

                ViewData["itemsPerPage"] = new SelectList(new[] { "6", "9", "12" }.Select(x => new { value = x, text = x }),
                "value", "text", itemsPerPage);

                if (Request.IsAjaxRequest())
                    return PartialView("PromotionProductList", promotedProducts);


                return View(promotedProducts);
            }
        }



    }
}
