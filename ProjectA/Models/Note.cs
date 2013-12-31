using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectA.Models
{
    public class Note
    {
        public String note { get; set; }
        [Key]
        public DateTime noteDate { get; set; }
        public string poster { get; set; }
    }
}