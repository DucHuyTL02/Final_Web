using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_bán_hàng__đồ_án_.Models.ViewModel
{
    public class SearchProductVN
    {
        public string SearchTerm { get; set; }
        public decimal? MinPrice { get; set; } // Giá thấp nhất
        public decimal? MaxPrice { get; set; } // Giá cao nhất
        public string SortOrder { get; set; }
        public int? CategoryID { get; set; }
        public string SelectedBrand { get; set; } // Thương hiệu được chọn
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
        public IPagedList<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<string> Brands { get; set; } // Danh sách thương hiệu
    }
}