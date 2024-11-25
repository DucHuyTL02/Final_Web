using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Web_bán_hàng__đồ_án_.Models.ViewModel
{
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }
        public int PageSize {  get; set; } =10;
        public List<Product> Products { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public PagedList.IPagedList<Product> NewProducts  { get; set; }
        public string CategoryName { get; set; }
    }
}