using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using Webdiyer.WebControls.Mvc;

namespace ProjectA.Controllers.backEnd
{
    public class AdminCustomerController : Controller
    {
        ProjectDBContext db = new ProjectDBContext();
        //
        // GET: /AdminCustomer/

        public ActionResult Index(int id = 1)
        {
            PagedList<Register> customers = db.register.OrderBy(p => p.userId).ToPagedList(id, 5);
            if (Request.IsAjaxRequest())
                return PartialView("CustomersList", customers);

            return View(customers);
        }

        //GET:/AdminCustomer/Detail?userId=12
        public ActionResult Detail(int userId)
        {
            Register customer = db.register.Find(userId);
            return View(customer);
        }


        public ActionResult CustomerSearchResult(String customerSearch, int id = 1)
        {

            int searchForId = 0;
            try
            {
                searchForId = Convert.ToInt32(customerSearch);
            }
            catch { }

            using (var db = new ProjectDBContext())
            {
                PagedList<Register> customerSearchResult = db.register.Where(p => p.firstName.Contains(customerSearch) || p.lastName.Contains(customerSearch) || p.addressLine1.Contains(customerSearch)
                    || p.addressLine2.Contains(customerSearch) || p.city.Contains(customerSearch) || p.country.Contains(customerSearch) || p.email.Contains(customerSearch)
                     || p.phone.Contains(customerSearch) || p.state.Contains(customerSearch) || p.suburban.Contains(customerSearch) || p.userName.Contains(customerSearch) || p.userId == searchForId)
                     .OrderBy(p => p.userId).ToPagedList(id, 5);

                if (Request.IsAjaxRequest())
                    return PartialView("CustomerSearchResultList", customerSearchResult);

                return View(customerSearchResult);
            }
        }


        [HttpPost]
        public ActionResult DeleteCustomer(int userId)
        {
            Register customer = db.register.Find(userId);
            db.register.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
