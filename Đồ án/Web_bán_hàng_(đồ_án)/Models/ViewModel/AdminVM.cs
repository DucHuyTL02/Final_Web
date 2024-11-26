using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_bán_hàng__đồ_án_.Models.ViewModel
{
    public class AdminVM
    {
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public List<TopProductVm> TopSellingProducts
        {
            get; set;
        }
    }
}