using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_bán_hàng__đồ_án_.Models;
using Web_bán_hàng__đồ_án_.Models.ViewModel;

namespace Web_bán_hàng__đồ_án_.Controllers
{
    public class ShoppingCartController : Controller
    {
        private LTWEntities db = new LTWEntities();

        // GET: ShoppingCart
        public ActionResult Index()
        {
            List<CartItem> ShoppingCart = Session["Cart"] as List<CartItem>;
            return View(ShoppingCart);
        }

        public ActionResult AddToCart(int ProductId)
        {
            ProductModel _db = new ProductModel();
            if (Session["Cart"] == null)
            {
                Session["Cart"] = new List<CartItem>();
            }

            List<CartItem> ShoppingCart = Session["Cart"] as List<CartItem>;
            if (ShoppingCart.FirstOrDefault(m => m.ProductID == ProductId) == null)
            {
                CartItem product = _db.FindProduct(ProductId);  // tim sp theo sanPhamID

                CartItem newItem = new CartItem()
                {
                    ProductID = ProductId,
                    ProductName = product.ProductName,
                    Quantity = 1,
                    ProductImage = product.ProductImage,
                    UnitPrice = product.UnitPrice,
                };

                ShoppingCart.Add(newItem);
            }
            else
            {
                // Nếu sản phẩm khách chọn đã có trong giỏ hàng thì không thêm vào giỏ nữa mà tăng số lượng lên.
                CartItem cardItem = ShoppingCart.FirstOrDefault(m => m.ProductID == ProductId);
                cardItem.Quantity++;
            }
            return RedirectToAction("Index", "ShoppingCart");
        }

        public RedirectToRouteResult UpdateAmount(int ProductId, int newAmount)
        {
            // tìm carditem muon sua
            List<CartItem> ShoppingCart = Session["Cart"] as List<CartItem>;
            CartItem EditAmount = ShoppingCart.FirstOrDefault(m => m.ProductID == ProductId);
            if (EditAmount != null)
            {
                EditAmount.Quantity = newAmount;
            }
            return RedirectToAction("Index");
        }

        public RedirectToRouteResult RemoveItem(int ProductId)
        {
            List<CartItem> shoppingCart = Session["Cart"] as List<CartItem>;
            CartItem DelItem = shoppingCart.FirstOrDefault(m => m.ProductID == ProductId);
            if (DelItem != null)
            {
                shoppingCart.Remove(DelItem);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Checkout()
        {
            List<CartItem> shoppingCart = Session["Cart"] as List<CartItem>;
            if (shoppingCart == null || shoppingCart.Count == 0)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new CheckoutVM
            {
                CartItems = shoppingCart,
                TotalAmount = shoppingCart.Sum(item => item.TotalPrice),
                OrderDate = DateTime.Now,
                ShippingAddress = customer.CustomerAddress,
                CustomerID = customer.CustomerID,
                Username = customer.Username
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null) return RedirectToAction("Login", "Account");

                var cart = Session["Cart"] as List<CartItem>;
                if (cart == null || !cart.Any()) { return RedirectToAction("Index", "Home"); }

                var customer = db.Customers.SingleOrDefault(c => c.Username == user.Username);
                if (customer == null) { return RedirectToAction("Login", "Account"); }

                // Determine payment status based on selected method
                string paymentStatus = "Chưa thanh toán";
                switch (model.PaymentMethod)
                {
                    case "Tiền mặt": paymentStatus = "Thanh toán tiền mặt"; break;
                    case "Paypal": paymentStatus = "Thanh toán paypal"; break;
                    case "Mua trước trả sau": paymentStatus = "Trả góp"; break;
                    default: paymentStatus = "Chưa thanh toán"; break;
                }

                var order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = model.OrderDate,
                    TotalAmount = model.TotalAmount,
                    PaymentStatus = paymentStatus,
                    OrderDetails = cart.Select(item => new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList()
                };

                db.Orders.Add(order);
                db.SaveChanges();

                // Clear the cart after placing the order
                Session["Cart"] = null;

                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }

            return View(model);
        }
    }

}