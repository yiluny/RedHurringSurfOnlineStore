using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectA.Models
{
    public class Images
    {
        [Key]
        public Guid ImageId { get; set; }
        

        [MaxLength]
        public byte[] Image { get; set; }


        public bool? IfCoverPic { get; set; }


        public int productId { get; set; }
        public virtual ProductDB product{ get; set; }

        //public IList<OrderDetail> orderDetails { get; set; }
    }
}