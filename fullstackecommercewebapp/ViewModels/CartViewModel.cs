using fullstackecommercewebapp.Models;

namespace fullstackecommercewebapp.ViewModels
{
    public class CartViewModel
    {
        public CartViewModel()
        {
            products = new List<Product>();
            cartItems = new List<CartItem>();
        }
        public IEnumerable<Product> products;
        public IEnumerable<CartItem> cartItems;

    }
}
