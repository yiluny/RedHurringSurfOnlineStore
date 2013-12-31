using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using System.Data;
using Webdiyer.WebControls.Mvc;
using ProjectA.ViewModels;

namespace ProjectA.Controllers.backEnd
{
    [ValidateInput(false)]
    public class AdminProductController : Controller
    {

        public ProjectDBContext db = new ProjectDBContext();

        //
        // GET: /AdminProduct/
        public ActionResult Index(int id = 1)
        {
            PagedList<ProductDB> products = db.Products.OrderByDescending(p => p.productId).ToPagedList(id, 5);
            if (Request.IsAjaxRequest())
                return PartialView("ProductsAdminList", products);

            return View(products);
        }

        public ActionResult promotionProduct(int id = 1)
        {
            PagedList<ProductDB> products = db.Products.Where(p => p.ifPromotion == true).OrderBy(p => p.productId).ToPagedList(id, 20);
            if (Request.IsAjaxRequest())
                return PartialView("ProductsAdminList", products);

            return View(products);
        }

        //GET:/AdminProduct/Detail?productId=12
        public ActionResult Detail(int productId)
        {
            ProductDB product = db.Products.Include("SubCategory").Include("Images").Single(p => p.productId == productId);
            adminProductViewModel viewProduct = new adminProductViewModel()
            {
                productId = product.productId,
                productName = product.productName,
                productPrice = product.productPrice,
                productDescription = product.productDescription,
                ifNewArrival = product.ifNewArrival,
                ifTopSales = product.ifTopSales,
                ifPromoted = product.ifPromotion,
                PromotedPrice = product.PromotedPrice,
                stockNumber = product.stockNumber,
                MainCategoryName = product.SubCategory.MainCategoryName,
                SubCategoryName = product.SubCategory.SubcategoryName,
                images = product.images
            };

            var SubCategory = (from sc in db.SubCategories select sc.SubcategoryName).GroupBy(x => x).Select(p => p.FirstOrDefault());
            var MainCategory = (from sc in db.SubCategories select sc.MainCategoryName).GroupBy(x => x).Select(p => p.FirstOrDefault());
            var TrueOrFalseSelectionList = new[] {   
                                new TrueOrFalseSelection { value = true, Name = "Yes"}, 
                                new TrueOrFalseSelection { value = false, Name = "No"}, 
                             };

            ViewBag.TrueOrFalseSelectionList = new SelectList(TrueOrFalseSelectionList, "value", "Name");
            ViewBag.subCategory = new SelectList(SubCategory);
            ViewBag.mainCategory = new SelectList(MainCategory);

            return View(viewProduct);
        }




        //GET:/AdminProduct/ProductSearchResult?productSearch=12
        public ActionResult ProductSearchResult(String productSearch, int id = 1)
        {
            int searchForId = 0;
            double searchForPrice = 0.0;
            int searchForStock = 0;
            try
            {
                searchForId = Convert.ToInt32(productSearch);
                searchForPrice = Convert.ToDouble(productSearch);
                searchForStock = Convert.ToInt32(productSearch);
            }
            catch { }

            using (var db = new ProjectDBContext())
            {
                PagedList<ProductDB> productSearchResult = db.Products.Where(p => p.MainCategory.Contains(productSearch) || p.productDescription.Contains(productSearch) || p.productName.Contains(productSearch) || p.productId == searchForId || p.productPrice==searchForPrice|| p.stockNumber == searchForStock
                    || p.SubCategory.SubcategoryName.Contains(productSearch))
                     .OrderBy(p => p.productId).ToPagedList(id, 5);

                if (Request.IsAjaxRequest())
                    return PartialView("ProductSearchResultList", productSearch);

                return View(productSearchResult);
            }
        }


        public void SetViewData(AddNewProductViewModel model)
        {
            if (model.MainCategory == null)
            {
                ViewBag.subCategory = new SelectList(db.SubCategories.Where(x => x.MainCategoryName == null), "SubcategoryName", "SubcategoryName", model.SubCategory);
            }
            else
            {
                ViewBag.subCategory = new SelectList(db.SubCategories.Where(x => x.MainCategoryName.Equals(model.MainCategory) && !x.SubcategoryName.Equals("Gift Card")), "SubcategoryName", "SubcategoryName", model.SubCategory);
            }

            ViewBag.mainCategory = new SelectList(db.SubCategories.Where(p => !p.SubcategoryName.Equals("Gift Card")).GroupBy(x => x.MainCategoryName).Select(p => p.FirstOrDefault()), "MainCategoryName", "MainCategoryName", model.MainCategory);


            ViewBag.ifNewArrival = new SelectList(new[]
                                          {
                                              new {Value="false",Text="No"},
                                              new{Value="true",Text="Yes"},
                                          },
                            "Value", "Text");


            ViewBag.ifTopSales = new SelectList(new[]
                                          {
                                              new {Value="false",Text="No"},
                                              new{Value="true",Text="Yes"},
                                          },
                            "Value", "Text");


            ViewBag.ifPromotion = new SelectList(new[]
                                          {
                                              new {Value="false",Text="No"},
                                              new{Value="true",Text="Yes"},
                                          },
                            "Value", "Text");

        }


        public ActionResult AddNewProduct()
        {
            AddNewProductViewModel model = new AddNewProductViewModel();

            SetViewData(model);

            return View();
        }


        [HttpPost]
        public ActionResult AddNewProduct(FormCollection collection, IEnumerable<HttpPostedFileBase> imgFiles, AddNewProductViewModel addNew)
        {

            TryUpdateModel(addNew, collection);

            if (addNew.ifPromotion != null && Convert.ToBoolean(addNew.ifPromotion))
            {
                if (addNew.PromotedPrice == null || String.IsNullOrWhiteSpace(Convert.ToString(addNew.productPrice)))
                {
                    ModelState.AddModelError("PromotedPrice", "Promoted price can not be empty");


                    SetViewData(addNew);
                    return View(addNew);

                }
            }

            if(imgFiles.FirstOrDefault() == null)
            {
                TempData["AddResult"] = false;
                TempData["Message"] = "At least one image must be uploaded";

                SetViewData(addNew);
                return View(addNew);
            }

            if (ModelState.IsValid && imgFiles.FirstOrDefault() != null)
            {

                var mainCate = (String)collection["MainCategory"];
                var subCate = (String)collection["SubCategory"];


                var subCateId = (from sc in db.SubCategories
                                 where sc.MainCategoryName.Equals(mainCate) &&
                             sc.SubcategoryName.Equals(subCate)
                                 select sc.SubCategoryId);

                int imgNumber = 0;
                foreach (var imageFile in imgFiles)
                {
                    if (imageFile != null)
                    {
                        if (imageFile.ContentLength > 0)
                        {
                            var imgByte = new byte[imageFile.ContentLength];
                            imageFile.InputStream.Read(imgByte, 0, imageFile.ContentLength);


                            Images img = new Images()
                            {
                                ImageId = Guid.NewGuid(),
                                Image = imgByte
                            };


                            if (imgNumber == 0)
                            {
                                img.IfCoverPic = Convert.ToBoolean(true);
                            }
                            else
                            {
                                img.IfCoverPic = Convert.ToBoolean(false);
                            }
                            db.image.Add(img);
                        }
                        imgNumber++;
                    }
                    
                        

                }



                ProductDB pro = new ProductDB()
                {
                    productName = collection["productName"],
                    productPrice = Convert.ToDouble(collection["productPrice"]),
                    productDescription = collection["productDescription"],
                    stockNumber = Convert.ToInt32(collection["stockNumber"]),
                    ifNewArrival = Convert.ToBoolean(collection["ifNewArrival"]),
                    ifTopSales = Convert.ToBoolean(collection["ifTopSales"]),
                    ifPromotion = Convert.ToBoolean(collection["ifPromotion"]),
                    PromotedPrice = Convert.ToDouble(collection["promotedPrice"]),
                    MainCategory = collection["MainCategory"],
                    SubCategoryId = Convert.ToInt32(subCateId.FirstOrDefault())

                };

                db.Products.Add(pro);

                TempData["AddResult"] = true;
                TempData["Message"] = "New product has been successfully added";

                db.SaveChanges();

                SetViewData(addNew);


                return RedirectToAction("AddNewProduct");
            }
            else
            {
                ViewData["AddResult"] = null;

                SetViewData(addNew);
                return View(addNew);
            }


        }


        public adminProductViewModel setDetailData(int productId) 
        {
            ProductDB product = db.Products.Include("SubCategory").Include("Images").Single(p => p.productId == productId);
            adminProductViewModel viewProduct = new adminProductViewModel()
            {
                productId = product.productId,
                productName = product.productName,
                productPrice = product.productPrice,
                productDescription = product.productDescription,
                ifNewArrival = product.ifNewArrival,
                ifTopSales = product.ifTopSales,
                ifPromoted = product.ifPromotion,
                PromotedPrice = product.PromotedPrice,
                stockNumber = product.stockNumber,
                MainCategoryName = product.SubCategory.MainCategoryName,
                SubCategoryName = product.SubCategory.SubcategoryName,
                images = product.images
            };

            var SubCategory = (from sc in db.SubCategories select sc.SubcategoryName).GroupBy(x => x).Select(p => p.FirstOrDefault());
            var MainCategory = (from sc in db.SubCategories select sc.MainCategoryName).GroupBy(x => x).Select(p => p.FirstOrDefault());
            var TrueOrFalseSelectionList = new[] {   
                                new TrueOrFalseSelection { value = true, Name = "Yes"}, 
                                new TrueOrFalseSelection { value = false, Name = "No"}, 
                             };

            ViewBag.TrueOrFalseSelectionList = new SelectList(TrueOrFalseSelectionList, "value", "Name");
            ViewBag.subCategory = new SelectList(SubCategory);
            ViewBag.mainCategory = new SelectList(MainCategory);

            return viewProduct;
        }



        //Get post detail
        //AdminProduct/Detail
        [HttpPost]
        public ActionResult Detail(int productId, FormCollection collection, IEnumerable<HttpPostedFileBase> imgFiles,adminProductViewModel ad)
        {
            

            ProductDB UpadatingProduct = (from p in db.Products where p.productId == productId select p).First();
            var SubCategory = (from sc in db.SubCategories select sc.SubcategoryName).GroupBy(x => x).Select(p => p.FirstOrDefault());
            var MainCategory = (from sc in db.SubCategories select sc.MainCategoryName).GroupBy(x => x).Select(p => p.FirstOrDefault());
            var TrueOrFalseSelectionList = new[] {   
                                new TrueOrFalseSelection { value = true, Name = "Yes"}, 
                                new TrueOrFalseSelection { value = false, Name = "No"}, 
                             };

            ViewBag.TrueOrFalseSelectionList = new SelectList(TrueOrFalseSelectionList, "value", "Name");
            ViewBag.subCategory = new SelectList(SubCategory);
            ViewBag.mainCategory = new SelectList(MainCategory);

            int imgNumber = 0;

            if (ModelState.IsValid)
            {

                var mainCate = (String)collection["MainCategoryName"];
                var subCate = (String)collection["SubCategoryName"];


                var subCateId = (from sc in db.SubCategories
                                 where sc.MainCategoryName.Equals(mainCate) &&
                             sc.SubcategoryName.Equals(subCate)
                                 select sc.SubCategoryId);

                imgNumber = 0;
                foreach (var imageFile in imgFiles)
                {
                    if (imageFile != null)
                    {
                        if (imageFile.ContentLength > 0 && collection["imgFilesId[" + imgNumber + "]"] != null)
                        {
                            var imgByte = new byte[imageFile.ContentLength];
                            imageFile.InputStream.Read(imgByte, 0, imageFile.ContentLength);

                            Guid ImgId = new Guid(collection["imgFilesId[" + imgNumber + "]"]);
                            Images img = (from i in db.image where i.ImageId == ImgId select i).First();

                            img.Image = imgByte;


                            if (imgNumber == 0)
                            {
                                img.IfCoverPic = Convert.ToBoolean(true);
                            }
                            else
                            {
                                img.IfCoverPic = Convert.ToBoolean(false);
                            }
                        }
                        else if (imageFile.ContentLength > 0 && collection["imgFilesId[" + imgNumber + "]"] == null)
                        {
                            var imgByte = new byte[imageFile.ContentLength];
                            imageFile.InputStream.Read(imgByte, 0, imageFile.ContentLength);


                            Images img = new Images()
                            {
                                ImageId = Guid.NewGuid(),
                                Image = imgByte,
                                productId = productId
                            };


                            if (imgNumber == 0)
                            {
                                img.IfCoverPic = Convert.ToBoolean(true);
                            }
                            else
                            {
                                img.IfCoverPic = Convert.ToBoolean(false);
                            }
                            db.image.Add(img);
                        }
                    }
                    imgNumber++;
                }
                UpadatingProduct.productName = collection["productName"];
                UpadatingProduct.productPrice = Convert.ToDouble(collection["productPrice"]);
                UpadatingProduct.productDescription = collection["productDescription"];
                UpadatingProduct.stockNumber = Convert.ToInt32(collection["stockNumber"]);
                UpadatingProduct.ifNewArrival = Convert.ToBoolean(collection["ifNewArrival"]);
                UpadatingProduct.ifTopSales = Convert.ToBoolean(collection["ifTopSales"]);
                UpadatingProduct.ifPromotion = Convert.ToBoolean(collection["ifPromoted"]);
                UpadatingProduct.PromotedPrice = Convert.ToDouble(collection["promotedPrice"]);
                UpadatingProduct.MainCategory = collection["MainCategoryName"];
                UpadatingProduct.SubCategoryId = Convert.ToInt32(subCateId.FirstOrDefault());


                db.SaveChanges();

                TempData["AddResult"] = "The product has been successfully updated";
            }
            ad = setDetailData(productId);
            return View(ad);
        }


        /*-------------------------------
          Dynamic change of dropdown list
         ---------------------------------*/
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult change(string mainCategory)
        {
            var subCategory = from a in db.SubCategories where a.MainCategoryName == mainCategory select a.SubcategoryName;
            var subCategoryList = subCategory.ToList();
            var subCategoryData = subCategoryList.Select(a => new SelectListItem() { Value = a, Text = a });
            return Json(subCategoryData, JsonRequestBehavior.AllowGet);
        }


        //Delete the product
        public ActionResult DeleteProduct(int productId)
        {
            ProductDB product = db.Products.Find(productId);
            var imagesId = from i in db.image where i.productId.Equals(productId) select i.ImageId;
            foreach (var id in imagesId)
            {
                Images image = db.image.Find(id);
                db.image.Remove(image);
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        [HttpPost]
        public ActionResult DeleteProduct(IEnumerable<String> products)
        {
            foreach (var r in products)
            {

                if (!Convert.ToString(r).Equals("false"))
                {
                    int deleteId = Convert.ToInt32(r.ToString());
                    ProductDB product = db.Products.Find(deleteId);
                    db.Products.Remove(product);

                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }



       
        public ActionResult DeleteImg(Guid imgId)
        {
            Images image = db.image.Find(imgId);
            db.image.Remove(image);
            db.SaveChanges();

            return Json("One existing image has been removed");
        }


    }
}
