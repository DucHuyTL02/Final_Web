using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_bán_hàng__đồ_án_.Models;
using Web_bán_hàng__đồ_án_.Models.ViewModel;

namespace Web_bán_hàng__đồ_án_.Controllers
{
    public class HomeController : Controller
    { 
        LTWEntities csdl = new LTWEntities();
        public ActionResult trangchu(string searchTerm ,int? page)
        { 
            var model = new HomeProductVM();
            var products = csdl.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) || p.ProductDescription.Contains(searchTerm) || p.Category.CategoryName.Contains(searchTerm));
            }
            int pageNumber = page ?? 1;
            int pageSize = 4;
            model.FeaturedProducts = products.OrderByDescending(p=>p.OrderDetails.Count()).Take(8).ToList();
            model.NewProducts=products.OrderBy(p => p.OrderDetails.Count()).Take(10).ToPagedList(pageNumber, pageSize);
            return View(model);

        }
        public ActionResult ProductDetails(int? id,int? quantity,int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = csdl.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ProductDetailsVM model = new ProductDetailsVM();
            model.product = product;
            return View(model);
        }
        public ActionResult FilterWatches(int? category, string filterType, string filterValue, string giua)
        {
            var watches = csdl.Products.AsQueryable();
            if (category.HasValue)
            {
                watches = watches.Where(w => w.CategoryID == category.Value);
            }

            if (!string.IsNullOrEmpty(giua) && giua == "Khoảng giá")
            {
                if (filterValue == "Dưới 2 Triệu")
                    watches = watches.Where(w => w.ProductPrice < 2000000);
                else if (filterValue == "Từ 2-6 Triệu")
                    watches = watches.Where(w => w.ProductPrice >= 2000000 && w.ProductPrice <= 6000000);
                else if (filterValue == "Trên 6 Triệu")
                    watches = watches.Where(w => w.ProductPrice > 6000000);
            }


            if (!string.IsNullOrEmpty(filterType) && filterType == "Thương hiệu")
            {
                watches = watches.Where(w => w.BrandPro == filterValue);
            }


            ViewBag.FilterType = filterType;
            ViewBag.FilterValue = filterValue;
            ViewBag.Giua = giua;
            return View(watches.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult MenWatches()
        {

            return View(csdl.Products.ToList());
        }
        public ActionResult WomenWatches()
        {

            return View();
        }
        public ActionResult CoupleWatches()
        {

            return View();
        }
        public ActionResult Banner()
        {
            return View();
        }
    }
}