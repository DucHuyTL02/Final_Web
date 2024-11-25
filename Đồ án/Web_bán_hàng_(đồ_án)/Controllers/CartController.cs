using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_bán_hàng__đồ_án_.Models;
using Web_bán_hàng__đồ_án_.Models.ViewModel;

namespace Web_bán_hàng__đồ_án_.Controllers
{
    public class CartController : Controller
    { 
        private LTWEntities db = new LTWEntities();
        private CartService GetCartService()
        {
            return new CartService(Session);
        }
        [Authorize]
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }
        public ActionResult AddToCart(int id, int quantity = 2)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                // Nếu không tìm thấy sản phẩm, trả về lỗi
                return HttpNotFound("Sản phẩm không tồn tại.");
            }

            var cartService = GetCartService();
            cartService.GetCart().AddItem(
                product.ProductID,
                product.ProductImage,
                product.ProductName,
                product.ProductPrice,
                quantity
            );

            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(int id)
        {
            var cartSevice=GetCartService();
            cartSevice.GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }
        [HttpPost]  
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cartService = GetCartService();
            cartService.GetCart().UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }

    }
}