using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProjectA.Models
{
    public class Cart
    {
        [Key]
        public int RecordId { get; set; }
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public double ProductSubTotal { get; set; }
        public string ProductSize { get; set; }
        public System.DateTime DateCreated { get; set; }
        public virtual ProductDB Product { get; set; }

    }
}