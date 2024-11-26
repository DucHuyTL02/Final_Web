using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_bán_hàng__đồ_án_.Models.ViewModel
{
    public class TopProductVm
    {
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}