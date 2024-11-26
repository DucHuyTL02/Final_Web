using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_bán_hàng__đồ_án_.Models;
using Web_bán_hàng__đồ_án_.Models.ViewModel;
using System.Security.Cryptography;  
using System.Text;
using System.Web.Security;


namespace Web_bán_hàng__đồ_án_.Controllers
{
    public class AccountController : Controller
    {
       LTWEntities db = new LTWEntities();
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // kiem tra ten dang nhap
                var existingUser = db.Users.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại!");
                    return View(model);
                }

                // neu chua ton tai thi tao ban ghi thong tin tk trong bang user
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password,
                    UserRole = "C"

                };
                db.Users.Add(user);
                // va tao ban ghi thong tin khach hang trong bang customer
                var customer = new Customer
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress,
                    Username = model.Username,
                };
                db.Customers.Add(customer);
                // luu thong tin tai khoan vao csdl
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi khi lưu dữ liệu: " + ex.Message);
                }
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login( LoginMV model)
        {
            if (ModelState.IsValid)
            {
                
                var existingUser = db.Users.SingleOrDefault(u => u.Username == model.Username && u.Password==model.Password);
                if (existingUser != null)
                {
                    Session["Username"] = existingUser.Username;
                    Session["UserRole"] = existingUser.UserRole;
                    FormsAuthentication.SetAuthCookie(existingUser.Username, false);
                    if (existingUser.UserRole == "A")
                    {
                        return RedirectToAction("TrangChuAdmin", "AdminHome", new { area = "Admin" });
                    }
                    else
                    {
                        Session["Login"] = true;
                        return RedirectToAction("trangchu", "Home");
                    }
                    
                }
                else
                {
                    ModelState.AddModelError(" ", "Tên đăng nhập hoặc mật khẩu không đúng");
                }
            }
            return View(model);
        }
      
        
        public ActionResult Logout()
        {
            // Xóa toàn bộ Session
            Session.Clear();
            Session.Abandon();

            // Chuyển hướng về trang chủ
            return RedirectToAction("trangchu", "Home");
        }
        public ActionResult History()
        {
            string username = Session["Username"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = db.Orders
                .Where(o => o.Customer.Username == username)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }
        public ActionResult HistoryDetails(int id)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Tải các OrderDetails kèm theo Product
            var orderDetails = db.OrderDetails
                .Where(od => od.OrderID == id)
                .Select(od => new
                {
                    Product = od.Product,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList();

            // Gán lại danh sách orderDetails vào order hoặc view model
            ViewBag.OrderDetails = orderDetails;

            return View(order);
        }
        public ActionResult Profile()
        {
            using (var db = new LTWEntities())
            {
                // Lấy thông tin username từ session hoặc User.Identity.Name
                var username = User.Identity.Name;

                // Lấy thông tin khách hàng
                var customer = db.Customers
                    .Where(c => c.Username == username)
                    .Select(c => new UserCustomerVM
                    {
                        Username = c.Username,
                        CustomerName = c.CustomerName,
                        CustomerPhone = c.CustomerPhone,
                        CustomerEmail = c.CustomerEmail,
                        CustomerAddress = c.CustomerAddress
                    })
                    .FirstOrDefault();

                if (customer == null)
                {
                    return HttpNotFound("Không tìm thấy thông tin khách hàng.");
                }

                return View(customer);
            }
        }

        [HttpGet]
        public ActionResult EditProfile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            using (var db = new LTWEntities())
            {
                var username = User.Identity.Name;

                var customer = db.Customers
                    .Where(c => c.Username == username)
                    .Select(c => new UserCustomerVM
                    {
                        Username = c.Username,
                        CustomerName = c.CustomerName,
                        CustomerPhone = c.CustomerPhone,
                        CustomerEmail = c.CustomerEmail,
                        CustomerAddress = c.CustomerAddress
                    })
                    .FirstOrDefault();

                if (customer == null)
                {
                    return HttpNotFound("Không tìm thấy thông tin khách hàng.");
                }

                return View(customer);
            }
        }

        [HttpPost]
        public ActionResult EditProfile(UserCustomerVM model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            using (var db = new LTWEntities())
            {
                if (ModelState.IsValid)
                {
                    var customer = db.Customers.FirstOrDefault(c => c.Username == model.Username);

                    if (customer != null)
                    {
                        customer.CustomerName = model.CustomerName;
                        customer.CustomerPhone = model.CustomerPhone;
                        customer.CustomerEmail = model.CustomerEmail;
                        customer.CustomerAddress = model.CustomerAddress;

                        db.SaveChanges();
                        TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        return HttpNotFound("Không tìm thấy thông tin khách hàng.");
                    }
                }

                return View(model);
            }
        }

    }

}