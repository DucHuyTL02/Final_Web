using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_bán_hàng__đồ_án_.Models.ViewModel
{
    public class OrderSearchVM
    {
        public int? OrderID { get; set; } // Tìm kiếm theo mã đơn hàng
        public string CustomerName { get; set; } // Tìm kiếm theo tên khách hàng
        public string SearchTerm { get; set; } // Tìm kiếm chung
        public string SortOrder { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
        public IPagedList<Order> Orders { get; set; } // Danh sách phân trang

    }
}