using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_bán_hàng__đồ_án_.Models;
using Web_bán_hàng__đồ_án_.Models.ViewModel;
using PagedList;
namespace Web_bán_hàng__đồ_án_.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private LTWEntities db = new LTWEntities();

        // GET: Admin/Products
        public ActionResult Product(string searchTerm ,decimal? minPrice, decimal? maxPrice,string sortOrder,int? page)
        {
            var model = new SearchProductVN();
            var products = db.Products.AsQueryable();
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

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,StockQuantityPro,BrandPro")] Product product, HttpPostedFileBase UploadImage, string ImageURL)
        {
            if (ModelState.IsValid)
            {
                if (UploadImage != null && UploadImage.ContentLength > 0)
                {
                    // Xử lý khi người dùng chọn file upload
                    var fileName = Path.GetFileName(UploadImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    UploadImage.SaveAs(path);
                    product.ProductImage = "/Uploads/" + fileName; // Đường dẫn lưu trong database
                }
                else if (!string.IsNullOrEmpty(ImageURL))
                {
                    // Xử lý khi người dùng nhập URL
                    product.ProductImage = ImageURL;
                }
                else
                {
                    // Nếu cả hai đều null, báo lỗi
                    ModelState.AddModelError("", "Vui lòng chỉ được chọn một");
                    ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
                    return View(product);
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Product"); // Chuyển hướng sau khi thêm thành công
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,StockQuantityPro,BrandPro")] Product product, HttpPostedFileBase UploadImage, string ImageURL)
        {
            if (ModelState.IsValid)
            {
                if (UploadImage != null && UploadImage.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(UploadImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
                    UploadImage.SaveAs(path);
                    product.ProductImage = "/Uploads/" + fileName;
                }
                else if (!string.IsNullOrEmpty(ImageURL))
                {
                    product.ProductImage = ImageURL;
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);

        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Product");
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
