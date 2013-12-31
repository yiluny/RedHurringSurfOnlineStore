using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using Webdiyer.WebControls.Mvc;


namespace ProjectA.Controllers
{
    public class ProductController : Controller
    {
        ProjectDBContext storeDB = new ProjectDBContext();
        //
        // GET: /Store/Browse?subName=T-shirt

        public ActionResult Browse(int SubCategoryId, int itemPerPage=6, int id = 1)
        {

            // Retrieve Genre and its Associated Products from database
            using (var db = new ProjectDBContext())
            {
                PagedList<ProductDB> productCategory = db.Products.Where(c => c.SubCategoryId == SubCategoryId).OrderBy(o => o.productId).ToPagedList(id, itemPerPage);

                ViewData["itemsPerPage"] = new SelectList(new[] { "6", "9", "12" }.Select(x => new { value = x, text = x }),
                "value", "text", itemPerPage);

                if (Request.IsAjaxRequest())
                    return PartialView("ProductList", productCategory);

                
         
                return View(productCategory); 
            }
           
        }

 

        //
        //GET: /Store/Details
        public ActionResult Details(int id)
        {
            var product = storeDB.Products.Include("SubCategory").Include("Images").Single(p => p.productId == id);


            return View(product);
        }

        public ActionResult getProductImg(Guid data)
        {
            var newArrivalImage = (from p in storeDB.image where p.ImageId == data select p).ToList();
            if (newArrivalImage.FirstOrDefault().Image != null)
                return new FileContentResult(newArrivalImage.FirstOrDefault().Image, "image/jpeg");
            else
                return null;
        }



        //
        //GET: /Store/SubCategoryMenu
        [ChildActionOnly]
        public ActionResult SubCategoryMenu()
        {
            string[] MainCategories = { "Headwear", "Socks", "Backpacks", "Skateboard", "Mens tee", "Wetsuits", "Shoes", "Thongs" };
            var SubCateNames = storeDB.SubCategories.ToList();
            var MainCategory = (from c in storeDB.SubCategories select c.MainCategoryName).Distinct().ToList();
            ViewBag.MainCategories = MainCategories;
            return PartialView(SubCateNames);
        }



        [HttpPost]
        public ActionResult getSearch(String userInput) 
        {
            var options = (from p in storeDB.Products where p.productName.Contains(userInput)  select p.productName).ToList();

            return Json(options);
        }


        
    }
}
