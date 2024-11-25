using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_bán_hàng__đồ_án_.Models.ViewModel
{
    public class ProductModel
    {
        private LTWEntities db;

        public ProductModel()
        {
            db = new LTWEntities();
        }

        public List<CartItem> FindAll()
        {
            return db.Products
                     .Select(p => new CartItem
                     {
                         ProductID = p.ProductID,
                         ProductName = p.ProductName,
                         UnitPrice = p.ProductPrice,
                         ProductImage = p.ProductImage
                     })
                     .ToList();
        }

        public CartItem FindProduct(int id)
        {
            var product = db.Products.SingleOrDefault(p => p.ProductID == id);
            if (product == null) return null;

            return new CartItem
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                UnitPrice = product.ProductPrice,
                ProductImage = product.ProductImage
            };
        }
    }
}