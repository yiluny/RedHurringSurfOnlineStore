using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectA.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public long userId { get; set; }
        public string userName { get; set; }
        public double totalPrice { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string reasons { get; set; }

        public IList<OrderDetail> OrderDetails { get; set; }
        //public IList<ProductDB> products { get; set; }
        //public IList<Images> Images { get; set; }
    }
}