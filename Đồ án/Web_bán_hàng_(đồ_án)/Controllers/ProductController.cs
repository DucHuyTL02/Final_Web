using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Web_bán_hàng__đồ_án_.Models;
using Web_bán_hàng__đồ_án_.Models.ViewModel;
using PagedList;

namespace Web_bán_hàng__đồ_án_.Controllers
{
    public class ProductController : Controller
    {
        LTWEntities csdl = new LTWEntities();
        // GET: Product
        public ActionResult ProductDetails(int? id, int? quantity, int? page)
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
        public ActionResult ProductList(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new SearchProductVN();
            var products = csdl.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm) || p.ProductDescription.Contains(searchTerm) || p.Category.CategoryName.Contains(searchTerm));
            }
            if (minPrice.HasValue)
            {
                model.MinPrice = minPrice.Value;
                products = products.Where(p => p.ProductPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                model.MaxPrice = maxPrice.Value;
                products = products.Where(p => p.ProductPrice <= maxPrice.Value);
            }
            switch (sortOrder)
            {
                case "price_Asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_Desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                case "name_Asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "name_Desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
            model.SortOrder = sortOrder;
            int pageNumber = page ?? 1;
            int pageSize = 8;
            model.Products = products.ToPagedList(pageNumber, pageSize);
            return View(model);
        }
        public ActionResult Category(int categoryId)
        {
            // Lấy danh sách sản phẩm theo danh mục
            var products = csdl.Products
                .Where(p => p.CategoryID == categoryId)
                .ToList();

            // Lấy thông tin danh mục để hiển thị tên trên View
            var category = csdl.Categories
                .FirstOrDefault(c => c.CategoryID == categoryId);

            if (category == null)
            {
                return HttpNotFound("Danh mục không tồn tại.");
            }

            // Truyền dữ liệu vào ViewModel (nếu cần thiết)
            var viewModel = new HomeProductVM()
            {
                Products = products,
                CategoryName = category.CategoryName
            };

            return View(viewModel);
        }

       
    }
}