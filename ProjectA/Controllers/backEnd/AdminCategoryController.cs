using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using ProjectA.ViewModels;
using Webdiyer.WebControls.Mvc;

namespace ProjectA.Controllers.backEnd
{
    public class AdminCategoryController : Controller
    {
        ProjectDBContext db = new ProjectDBContext();

        //
        // GET: /AdminCategory/

        public ActionResult Index()
        {
            //Get value for mainCategory Dropdown List
            var categories = (from subCat in db.SubCategories where !subCat.SubcategoryName.Equals("Gift Card") select subCat.MainCategoryName).GroupBy(s => s).Select(o => o.FirstOrDefault());
            ViewBag.Maincategories = new SelectList(categories);

            return View();
        }

        [HttpPost]
        public ActionResult changeSubcate(String mainCategoryName)
        {
            var subcate = (from s in db.SubCategories where s.MainCategoryName.Equals(mainCategoryName) select s).ToList();

            UpdateCategoryViewModel update = new UpdateCategoryViewModel();

            update.existingData = new List<SubCategory> { };
            foreach (var u in subcate)
            {
                    update.existingData.Add(u);
            }

            //return Json(subcate);
            return PartialView("changeSubcate",update);
        }



        [HttpPost]
        public ActionResult updateSubcate(FormCollection collection, String mainCategoryName, IEnumerable<String> newSubCate, UpdateCategoryViewModel subca)
        {
            int old = 0;
            int New = 0;
            if (ModelState.IsValid)
            {
                if (subca != null && subca.existingData!=null)
                {
                    foreach(var n in subca.existingData)
                    {
                        if (n != null) 
                        {
                            var newSubName = db.SubCategories.Find(n.SubCategoryId);
                            if (!newSubName.SubcategoryName.Equals(n.SubcategoryName))
                            {
                                old++;
                                newSubName.SubcategoryName = n.SubcategoryName;
                            }
                        }
                    }

                    db.SaveChanges();
                }
                

                if (newSubCate != null)
                {
                    foreach (var sub in newSubCate)
                    {
                        if (!String.IsNullOrWhiteSpace(sub))
                        {
                            New++;
                            SubCategory s = new SubCategory()
                            {
                                SubcategoryName = sub,
                                MainCategoryName = mainCategoryName
                            };

                            db.SubCategories.Add(s);
                        }
                    }

                    db.SaveChanges();
                }

                var categories = (from subCat in db.SubCategories select subCat.MainCategoryName).GroupBy(s => s).Select(o => o.FirstOrDefault());


                ViewBag.Maincategories = new SelectList(categories);


                if (old > 0 && New > 0)
                    ViewData["UpdateResult"] = old + " existing subcategories updated" + "<br />" + New + " new subcategories added";
                else if (old > 0 && New == 0)
                    ViewData["UpdateResult"] = old + " existing subcategories updated";
                else if(old==0 && New>0)
                    ViewData["UpdateResult"] = New + " new subcategories added";
            }

            var subcate = (from s in db.SubCategories where s.MainCategoryName == mainCategoryName select s).FirstOrDefault();
            return View("Index",subcate);
        }


        [HttpPost]
        public ActionResult deleteSubCatAndProducts(SubCategory subcate)
        {
            String subName = null;

            var group = (from p in db.Products where p.SubCategoryId == subcate.SubCategoryId select p).ToList();
            if (group != null)
            {
                foreach (var pro in group)
                {
                    var Productimages = (from i in db.image where i.productId == pro.productId select i).ToList();
                    if (Productimages != null)
                    {
                        foreach (var image in Productimages)
                        {
                            db.image.Remove(image);
                        }
                    }

                    db.Products.Remove(pro);
                }
            }

            SubCategory subCate = db.SubCategories.Find(subcate.SubCategoryId);
            subName = subCate.SubcategoryName;

            db.SubCategories.Remove(subCate);


            db.SaveChanges();

            ViewData["UpdateResult"] = subName + "and its products have been successfully deleted";

            var categories = (from subCat in db.SubCategories select subCat.MainCategoryName).GroupBy(s => s).Select(o => o.FirstOrDefault());
            ViewBag.Maincategories = new SelectList(categories);


            return View("Index",subcate);
        }
    }
}
