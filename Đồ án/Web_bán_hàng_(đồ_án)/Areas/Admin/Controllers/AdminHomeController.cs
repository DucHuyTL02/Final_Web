using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_bán_hàng__đồ_án_.Models;
using Web_bán_hàng__đồ_án_.Models.ViewModel;

namespace Web_bán_hàng__đồ_án_.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult TrangChuAdmin()
        {
            using (var db = new LTWEntities())
            {
                // Tổng số khách hàng
                int totalCustomers = db.Customers.Count();

                // Tổng số đơn hàng
                int totalOrders = db.Orders.Count();

                // Tổng số đơn hàng đã hoàn thành
                int completedOrders = db.Orders.Count(o => o.PaymentStatus == "Paid");

                // Tổng doanh thu
                decimal totalRevenue = db.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0;

                // Tổng số sản phẩm
                int totalProducts = db.Products.Count();

                // Sản phẩm bán chạy nhất
                var topSellingProducts = db.OrderDetails
                    .GroupBy(od => od.ProductID)
                    .Select(g => new TopProductVm
                    {
                        ProductName = g.FirstOrDefault().Product.ProductName,
                        QuantitySold = g.Sum(od => od.Quantity),
                        TotalRevenue = g.Sum(od => od.Quantity * od.UnitPrice)
                    })
                    .OrderByDescending(p => p.QuantitySold)
                    .Take(5) // Lấy 5 sản phẩm bán chạy nhất
                    .ToList();

                // Đổ dữ liệu vào ViewModel
                var model = new AdminVM
                {
                    TotalCustomers = totalCustomers,
                    TotalOrders = totalOrders,
                    CompletedOrders = completedOrders,
                    TotalRevenue = totalRevenue,
                    TotalProducts = totalProducts,
                    TopSellingProducts = topSellingProducts
                };

                return View(model);
            }
        }
    }
}