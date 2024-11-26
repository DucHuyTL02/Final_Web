using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_bán_hàng__đồ_án_.Models.ViewModel
{
    public class UserCustomerVM
    {
        public string Username { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public User User { get; set; }
        public Customer Customer { get; set; }
    }
}