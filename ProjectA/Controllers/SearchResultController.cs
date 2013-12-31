using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using Webdiyer.WebControls.Mvc;


namespace ProjectA.Controllers
{
    public class SearchResultController : Controller
    {

        public ProjectDBContext storedb = new ProjectDBContext();

        //
        // GET: /SearchResult/searchResult
        public ActionResult searchResult(String search, int id = 1)
        {
            ViewBag.content = search;

            using (var db = new ProjectDBContext())
            {
                PagedList<ProductDB> searchResults = db.Products.Where(p => p.productName.Contains(search) || p.productDescription.Contains(search)).OrderBy(p => p.productName).ToPagedList(id, 5);

                if (Request.IsAjaxRequest())
                    return PartialView("searchList", searchResults);

                return View(searchResults);
            }
        }

    }
}
