using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace ProjectA.Models
{
    [Bind(Exclude = "shopId")]
    public class shopInfo
    {
        [Key]
        public int shopId { get; set; }
        public string shopName { get; set; }
        public string shopDescription { get; set; }
        [DataType(DataType.MultilineText)]
        public string shopOpenTime { get; set; }
        public string shopAddress { get; set; }
        public string shopContactNum { get; set; }
    }
}