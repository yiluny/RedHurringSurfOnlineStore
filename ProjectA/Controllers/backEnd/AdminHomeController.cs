using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectA.Models;
using ProjectA.ViewModels;

namespace ProjectA.Controllers.backEnd
{
    public class AdminHomeController : Controller
    {
        ProjectDBContext db = new ProjectDBContext();
        //
        // GET: /AdminHome/

        public ActionResult Index()
        {
            DateTime nowTime = DateTime.Now;
            int thisYear = nowTime.Year;
            TimeSpan aWeek = new System.TimeSpan(7, 0, 0, 0, 0);
            DateTime startTime = nowTime.Subtract(aWeek);

            var Notes = (from n in db.Notes where n.noteDate >= startTime select n).OrderByDescending(n => n.noteDate);
            var orderYear = (from c in db.Orders where c.OrderDate.Year < thisYear select c.OrderDate.Year).Distinct().ToList();
            var DeleteNotes = (from n in db.Notes where n.noteDate <= startTime select n).OrderByDescending(n => n.noteDate);

            foreach (var note in DeleteNotes)
            {
                Note deleteNote = db.Notes.Find(note.noteDate);
                db.Notes.Remove(deleteNote);
            }
            db.SaveChanges();
            ViewBag.notes = Notes;
            ViewBag.orderYear = new SelectList(orderYear);
            return View();
        }


        //public ActionResult Category()
        //{
        //    return View();
        //}

        [HttpPost]
        public ActionResult PieChartResult(string date)
        {
            //retrive search time
            DateTime startTime;
            DateTime nowTime = DateTime.Now;
            TimeSpan aDay = new System.TimeSpan(1, 0, 0, 0, 0);
            TimeSpan aWeek = new System.TimeSpan(7, 0, 0, 0, 0);
            TimeSpan aFourWeeks = new System.TimeSpan(28, 0, 0, 0, 0);
            TimeSpan aYear = new System.TimeSpan(365, 0, 0, 0, 0);

            if (date.Equals("day"))
                startTime = nowTime.Subtract(aDay);
            else if (date.Equals("week"))
                startTime = nowTime.Subtract(aWeek);
            else if (date.Equals("month"))
                startTime = nowTime.Subtract(aFourWeeks);
            else
                startTime = nowTime.Subtract(aYear);

            var timedOrderDetails = (from od in db.Orders where od.OrderDate.CompareTo(startTime) >= 0 select od.OrderDetails);

            if (date.Equals("all"))
            {
                timedOrderDetails = (from od in db.Orders select od.OrderDetails);
            }

            //create a new data table for pie chart
            List<OrderDetailsWithSubCategory> odtails = new List<OrderDetailsWithSubCategory>();
            foreach (var orderDetailList in timedOrderDetails)
            {

                foreach (var orderDetail in orderDetailList)
                {
                    var OrderDetailsWithSubCategory = new OrderDetailsWithSubCategory()
                    {

                        orderdetail = orderDetail,
                        subcategoryName = orderDetail.Product.SubCategory.SubcategoryName,
                        mainCategoryName = orderDetail.Product.MainCategory
                    };
                    odtails.Add(OrderDetailsWithSubCategory);
                }
            }
            var groupedDetails = odtails.GroupBy(c => c.subcategoryName);

            //put all data together
            List<pieChartViewModel> pieChartData = new List<pieChartViewModel>();
            foreach (var groupedDetail in groupedDetails)
            {
                double sum = 0;
                foreach (var item in groupedDetail)
                {
                    sum += item.orderdetail.Quantity;
                }
                var singlePieData = new pieChartViewModel()
                {
                    subCategoryName = groupedDetail.First().subcategoryName,
                    mainCategoryName = groupedDetail.First().mainCategoryName,
                    sum = sum,
                };
                pieChartData.Add(singlePieData);
            }
            pieChartData.ToArray();
            return Json(pieChartData);
        }

        [HttpPost]
        public ActionResult LineChartResult(int yearTime)
        {
            DateTime nowTime = DateTime.Now;
            int thisYear = nowTime.Year;
            var thisYearOrders = (from order in db.Orders where order.OrderDate.Year.Equals(thisYear) select order).ToList();
            var pastYearOrders = (from order in db.Orders where order.OrderDate.Year.Equals(yearTime) select order).ToList();

            List<LineChartDataViewModel> wholeYearsOrdersSum = new List<LineChartDataViewModel>();

            var pastYearOrdersSum = new LineChartDataViewModel()
            {
                sum = getSingleMonthResult(pastYearOrders),
                year = yearTime
            };
            var thisYearOrdersSum = new LineChartDataViewModel()
            {
                sum = getSingleMonthResult(thisYearOrders),
                year = thisYear
            };

            wholeYearsOrdersSum.Add(pastYearOrdersSum);
            wholeYearsOrdersSum.Add(thisYearOrdersSum);

            return Json(wholeYearsOrdersSum);
        }

        private double[] getSingleMonthResult(List<Order> wholeYearOrders)
        {
            double[] twelveMonthsSum = new double[12];
            foreach (var order in wholeYearOrders)
            {
                var month = order.OrderDate.Month;
                twelveMonthsSum[month - 1] += order.totalPrice;
            }
            return twelveMonthsSum;
        }

        public ActionResult SaveNote(string note, string poster)
        {
            Note newNote = new Note()
            {
                note = note,
                noteDate = DateTime.Now,
                poster = poster
            };
            db.Notes.Add(newNote);
            db.SaveChanges();

            return Json(newNote);
        }

        public string formatADate(DateTime date)
        {
            var str = string.Format("{0:MM/dd/yyyy HH:mm:ss}", date);
            DateTime now = DateTime.Now;
            if (now.Day == date.Day)
                str = "Today " + string.Format("{0:HH:mm:ss}", date);
            return str;
        }

        //
        //GET: /AdminHome/SubCategoryMenu
        [ChildActionOnly]
        public ActionResult AdminPartialView()
        {
            var allProducts = (from p in db.Products select p).Count();
            var subcategories = (from p in db.SubCategories select p).Count();
            var promotionProducts = (from p in db.Products where p.ifPromotion == true select p).Count();
            var users = (from u in db.register select u).Count();
            var unshippedOrders = (from u in db.Orders where u.OrderStatus == "Pending" select u).Count();

            ViewBag.allProducts = allProducts;
            ViewBag.subcategories = subcategories;
            ViewBag.promotionProducts = promotionProducts;
            ViewBag.users = users;
            ViewBag.unshippedOrders = unshippedOrders;
            return PartialView();
        }
    }
}
