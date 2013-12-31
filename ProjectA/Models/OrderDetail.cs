using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace ProjectA.Models
{
    public class OrderDetail
    {
        [Key]
        public Guid OrderDetailId { get; set; }
        
        
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalCost { get; set; }
        public string reasons { get; set; }
        public bool? ifPackaged { get; set; }
        public string productSize { get; set; }

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int ProductId { get; set; }
        public virtual ProductDB Product { get; set; }
    }
}