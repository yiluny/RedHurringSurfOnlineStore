using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectA.Models
{
    public class Paypal
    {
        public Paypal()
        {
        }

        public string cmd { get; set; }
        public string business { get; set; }   
        public string no_shipping { get; set; }
        public string @return { get; set; }
        public string cancel_return { get; set; }
        public string notify_url { get; set; }
        public string currency_code { get; set; }
        public double shipping_1 { get; set; }
        

        public IList<orderInformation> orders { get; set; }
    }

    public class orderInformation 
    {
        public string item_name { get; set; }
        public int quantity { get; set; }
        public double unitPrice { get; set; }
        public double tax { get; set; }
    }
}