using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_bán_hàng__đồ_án_.Models.ViewModel;
using Web_bán_hàng__đồ_án_.Models;

namespace Web_bán_hàng__đồ_án_.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private LTWEntities db=new LTWEntities();
        [HttpGet]
        //Get: Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            //kiểm tra giỏ hàng trong section
            //nếu giỏ hàng rỗng hoặc không có sản phẩm thì chuyển hướng về trang chủ
            var cart = Session["Cart"] as Cart;
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("trangchu", "Home");
            }
            //Xác thực ng dùng đã đăng nhập chưa, nếu chưa thì chuyển hướng tới đănh nhập
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //lấy thông tin từ khách hàng từ CSDL, nếu chưa thì chueyenr hướng tới trang đăng nhập
            //nếu rồi thì lấy địa chỉ của khách hànng và gáng vào ShippingAddress của checkoutVM
            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");

            }
            var model = new CheckoutVM
            { //tạo dữ liệu hiển thị cho checkoutVm
                CartItems = cart.Items.ToList(),//Lấy danh sách các sản phẩm trong giỏ 
                TotalAmount = cart.Items.Sum(item => item.TotalPrice), // Tổng giá trị các mặt hàng trong giỏ
                OrderDate = DateTime.Now,//mặc định lấy bằng thời điểm đặt hàng
                ShippingAddress = customer.CustomerAddress,//Lấy địa chỉ mặc định từ bẳng customer
                CustomerID = customer.CustomerID,//lấy mã khách từ bảng Customer
                Username = customer.Username
            };
            return View(model);
        }

        //POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var cart = Session["Cart"] as Cart;//List<CartItem>;
                //Nếu giỏ hàng rổng chuyển tới trang Home
                if (cart == null || !cart.Items.Any()) { return RedirectToAction("trangchu", "Home"); }


                //Nếu ng dùng ch đăng nhập chuyển đén trang đăng nhập
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null) { return RedirectToAction("Login", "Account"); }


                //nếu khách hàng k khớp với tên đăng nhập sẽ chuyển đến trang đăng nhập login
                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null) { return RedirectToAction("Login", "Account"); }


                //Nếu ng dùng chọn thanh toán bằn Paypal, sẽ chuyển đến trang PaymentWithPaypal
                if (model.PaymentMethod == "Paypal")
                {
                    return RedirectToAction(" PaymentWithPaypal", "Paypal", model);
                }
                //Thiết lập PaypalStatus dựa trên PaymentMethod
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt": paymentStatus = "Thanh toán tiền mặt"; break;
                    case "Mua trước trả sau": paymentStatus = "Trả góp"; break;
                    default: paymentStatus = "Chưa thanh toán"; break;
                }
                //Tạo đơn hàng và chi tiết đơn hàng liên quan
                var order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = DateTime.Now,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = paymentStatus,
                    ShippingAddress=model.ShippingAddress,
                    OrderDetails = cart.Items.Select(item => new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };
                //Lưu đơn hàng vào CSDL 
                db.Orders.Add(order);
                db.SaveChanges();
                //Xóa giỏ hàng sau khi đặt hàng thành công  
                Session["Cart"] = null;
                //Chuyển đến trang xác nhận đơn hàng
                return RedirectToAction("OrderSuccess", new { orderId = order.OrderID });
            }
            return View(model);

        }
        public ActionResult OrderSuccess(int orderId)
        {
            // Tìm thông tin đơn hàng từ cơ sở dữ liệu
            var order = db.Orders.FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                return RedirectToAction("Index", "Home"); // Nếu không tìm thấy đơn hàng, quay về trang chủ
            }

            // Truyền thông tin qua ViewBag
            ViewBag.OrderID = order.OrderID;
            ViewBag.OrderDate = order.OrderDate;
            ViewBag.TotalAmount = order.TotalAmount;

            return View(); // Trả về View "PaymentConfirmation.cshtml"
        }

    }
}