using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Web_bán_hàng__đồ_án_.Models;
using Web_bán_hàng__đồ_án_.Models.ViewModel;

namespace Web_bán_hàng__đồ_án_.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private LTWEntities db = new LTWEntities();

        // GET: Admin/Users
        public ActionResult Index(string sortOrder, string search, string role)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var users = db.Users.AsQueryable();
            // Lọc theo vai trò (role)
            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(p => p.UserRole == role);
            }
            // Tìm kiếm theo từ khóa (username)
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(p => p.Username.Contains(search));
            }


            switch (sortOrder)
            {
                case "name_Asc":
                    users = users.OrderBy(p => p.Username);
                    break;
                case "name_Desc":
                    users = users.OrderByDescending(p => p.Username);
                    break;
            }
            ViewBag.RoleList = new SelectList(new[]
     {
        new { Value = "", Text = "Tất cả" },
        new { Value = "A", Text = "Admin" },
        new { Value = "U", Text = "User" }
    }, "Value", "Text", role);

            return View(users.ToList());
        }

        // GET: Admin/Users/Details/5
        public ActionResult Details(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tìm User
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Lấy thông tin Customer liên kết
            var customer = db.Customers.FirstOrDefault(c => c.Username == id);

            var viewModel = new UserCustomerVM
            {
                User = user,
                Customer = customer
            };

            return View(viewModel);

        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,UserRole")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Password,UserRole")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                var relatedCustomers = db.Customers.Where(c => c.Username == user.Username).ToList();
                foreach (var customer in relatedCustomers)
                {
                    db.Customers.Remove(customer);
                }

                db.Users.Remove(user);
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
