using fullstackecommercewebapp.Models;

namespace fullstackecommercewebapp.Repositories.IRepos
{
    public interface ICartItemRepo: IBaseRepo<Models.CartItem>
    {
        public void AddToCart(int cartId, int productId);
        public void RemoveFromCart(int cartId, int productId);

        public void Remove(int cartId, int productId);
        public void EmptyCart(int cartId);
        public void MigrateCart(int userId, int cartId);
        public IEnumerable<CartItem> ViewCart(int cartId);
    }
}
