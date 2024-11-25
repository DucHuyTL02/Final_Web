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
                
                var existingUser = db.Users.SingleOrDefault(u => u.Username == model.Username && u.Password==model.Password&& u.UserRole=="C");
                if (existingUser != null)
                {
                    Session["Username"] = existingUser.Username;
                    Session["UserRole"] = existingUser.UserRole;
                    FormsAuthentication.SetAuthCookie(existingUser.Username, false);
                    return RedirectToAction("trangchu", "Home");
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
    }
    
}