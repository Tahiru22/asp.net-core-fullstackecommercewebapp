using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.Repositories.IRepos;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;

namespace fullstackecommercewebapp.Repositories.Repos
{
    public class CartItemRepo: BaseRepo<CartItem>, ICartItemRepo
    {
        public CartItemRepo(AppDbContext db) : base(db)
        {

        }
        public void AddToCart(int cartId, int productId)
        {
            var cartItem = _db.CartItem.SingleOrDefault(
                c => c.CartId == cartId
                && c.ProductId == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    CartId = cartId,
                    Product = _db.Product.SingleOrDefault(
                    p => p.Id == productId),
                    Quantity = 1,
                    Price = _db.Product.Where(p => p.Id == productId).Select(p => p.Price).FirstOrDefault(),
                };
                _db.CartItem.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }
        }

        public void RemoveFromCart(int cartId, int productId)
        {
            var cartItem = _db.CartItem.SingleOrDefault(
                c => c.CartId == cartId
                && c.ProductId == productId);
            if (cartItem.Quantity == 1)
            {
                _db.CartItem.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity--;
            }
        }

        public void Remove(int cartId, int productId)
        {
            var cartItem = _db.CartItem.SingleOrDefault(
                c => c.CartId == cartId
                && c.ProductId == productId);
            _db.CartItem.Remove(cartItem);
        }
        public void EmptyCart(int cartId)
        {
            var cartItems = _db.CartItem.Where(c => c.CartId == cartId);
            foreach (var cartItem in cartItems)
            {
                _db.Remove(cartItem);
            }
        }

        public void MigrateCart(int userId, int cartId)
        {
            var items = _db.CartItem.Where(c => c.CartId == cartId);
            foreach (var item in items)
            {
                item.CartId = userId;
            }
            _db.SaveChanges();
        }

        public IEnumerable<CartItem> ViewCart(int cartId)
        {
            return _db.CartItem.Include(c => c.Product).Where(c => c.CartId == cartId);
        }
    }
}
