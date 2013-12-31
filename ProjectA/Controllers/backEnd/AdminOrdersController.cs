using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.ViewModels;
using ProjectA.Models;
using System.Globalization;
using System.Web.UI;
using Webdiyer.WebControls.Mvc;

namespace ProjectA.Controllers.backEnd
{
    [ValidateInput(false)]
    public class AdminOrdersController : Controller
    {
        public ProjectDBContext db = new ProjectDBContext();

        //
        // GET: /AdminOrders/
        public ActionResult Index(int id = 1)
        {
            PagedList<Order> orders = db.Orders.OrderByDescending(o => o.OrderDate).ToPagedList(id, 10);

            if (Request.IsAjaxRequest())
                return PartialView("OrdersList", orders);

            return View(orders);
        }


        //GET:/AdminCustomer/Detail?orderId
        public ActionResult Detail(Guid orderId, int id = 1)
        {

            PagedList<OrderDetail> orderDetails = (from od in db.OrderDetails
                                                   where od.OrderId == orderId
                                                   select od).OrderByDescending(od => od.Quantity).ToPagedList(id, 3);
            
            if (Request.IsAjaxRequest())
                return PartialView("OrderDetailList", orderDetails);

            ViewBag.reason = (from od in db.Orders where od.OrderId == orderId select od.reasons).FirstOrDefault();
            ViewBag.status = new SelectList(new[]
                                          {
                                              new {Value="Pending",Text="Pending"},
                                              new{Value="Shipped",Text="Shipped"},
                                          },
                            "Value", "Text");

            ViewBag.currentStatus = (from od in db.Orders where od.OrderId == orderId select od.OrderStatus).FirstOrDefault();

            return View(orderDetails);

        }


        [HttpPost]
        public ActionResult Detail(String reason, String OrderId, FormCollection collection)
        {
            Order singleOrder = db.Orders.Find(Guid.Parse(OrderId));
            singleOrder.reasons = reason;
            singleOrder.OrderStatus = Convert.ToString(collection["currentStatus"]);
            db.SaveChanges();


            return RedirectToAction("Index", "AdminOrders");
        }


        public ActionResult OrderSearchResult(String OrderSearch, int id = 1)
        {
            PagedList<Order> OrderSearchResult;

            Guid searchForGuId = Guid.Empty;
            try
            {
                searchForGuId = new Guid(OrderSearch);
            }
            catch { }

            int searchForUserId = 0;
            try
            {
                searchForUserId = Convert.ToInt32(OrderSearch);
            }
            catch { }

            DateTime searchForDateTime = DateTime.Now;
            try
            {
                searchForDateTime = DateTime.Parse(OrderSearch);
            }
            catch { }


            using (var db = new ProjectDBContext())
            {
                OrderSearchResult = db.Orders.Where(p => p.OrderStatus.Contains(OrderSearch) || p.userName.Contains(OrderSearch) || p.userId == searchForUserId || p.OrderId == searchForGuId || (p.OrderDate.Year == searchForDateTime.Year && p.OrderDate.Month == searchForDateTime.Month && p.OrderDate.Day == searchForDateTime.Day))
                     .OrderBy(p => p.OrderDate).ToPagedList(id, 5);

                if (Request.IsAjaxRequest())
                    return PartialView("OrderSearchResultList", OrderSearchResult);
                
                return View(OrderSearchResult);
            }
        }


        [HttpPost]
        public ActionResult packageGoods(Guid orderDetailId)
        {
            // Get the name of the product to display confirmation
            OrderDetail singleOrderDetail = db.OrderDetails.Find(orderDetailId);
            singleOrderDetail.ifPackaged = true;
            

            ProductDB singleProduct = db.Products.Find(singleOrderDetail.ProductId);
            singleProduct.stockNumber -= singleOrderDetail.Quantity;
            if (singleProduct.stockNumber <= 0 )
            {
                singleProduct.stockNumber = 0;
            }

            db.SaveChanges();

            
            var results = new PackagedGoods()
            {
                productId = singleOrderDetail.ProductId,
                productName = singleOrderDetail.Product.productName,
                productSize = singleOrderDetail.productSize,
                quantity = singleOrderDetail.Quantity
            };
            return Json(results);

        }



    }
}
