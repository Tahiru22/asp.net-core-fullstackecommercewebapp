using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace fullstackecommercewebapp.Controllers
{
    public class ShopController : BaseController
    {
        private readonly UserManager<User> _userManager;
        public ShopController(UserManager<User> userManger) : base()
        {
            _userManager = userManger;
        }
        public IActionResult Index(int? CurrentCategoryId, string LowPriceSearchString, string CurrentProductFilter, string CurrentCategoryFilter,
            string CurrentLowPriceFilter, string CurrentHighPriceFilter, string HighPriceSearchString, string ProductSearchString, string CategorySearchString, int? page)
        {
            if (ProductSearchString != null || LowPriceSearchString != null || HighPriceSearchString != null || CategorySearchString != null)
            {
                page = 1;
            }
            else
            {
                ProductSearchString = CurrentProductFilter;
                LowPriceSearchString = CurrentLowPriceFilter;
                HighPriceSearchString = CurrentHighPriceFilter;
                CategorySearchString = CurrentCategoryFilter;
            }
            ViewBag.CurrentProductFilter = ProductSearchString;
            ViewBag.CurrentLowPriceFilter = LowPriceSearchString;
            ViewBag.CurrentHighPriceFilter = HighPriceSearchString;
            ViewBag.CurrentCategoryFilter = CategorySearchString;

            ViewBag.categories = _uow.categoryRepo.getAll();

            IEnumerable<Product> products = _uow.productRepo.getWithCategories();
            if (!String.IsNullOrEmpty(CategorySearchString))
            {
                products = products.Where(p => p.Category.Name.ToLower() == CategorySearchString.ToLower());
            }
            if (!String.IsNullOrEmpty(ProductSearchString))
            {
                products = products.Where(s => s.Name.ToLower().Contains(ProductSearchString.ToLower()));
            }
            if (!String.IsNullOrEmpty(LowPriceSearchString))
            {
                bool isNumeric = double.TryParse(LowPriceSearchString, out _);
                if (isNumeric)
                {
                    products = products.Where(s => s.Price >= Convert.ToDouble(LowPriceSearchString));
                }
            }
            if (!String.IsNullOrEmpty(HighPriceSearchString))
            {
                bool isNumeric = double.TryParse(HighPriceSearchString, out _);
                if (isNumeric)
                {
                    products = products.Where(s => s.Price <= Convert.ToDouble(HighPriceSearchString));
                }
            }
            IEnumerable<CartItem> cartItems = _uow.cartItemRepo.ViewCart(getCartId());
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            ViewBag.cartItems = cartItems;
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public IActionResult getProductDetails(int id)
        {
            var product = _uow.productRepo.getById(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public int getCartId()
        {
            if (HttpContext.Session.GetInt32("CartId") == null)
            {
                if (!string.IsNullOrWhiteSpace(User.Identity.Name))
                {
                    var u = _uow.userRepo.getByUserName(User.Identity.Name);
                    HttpContext.Session.SetInt32("CartId", u.Id);
                }
                else
                {
                    int tempCartId = Guid.NewGuid().GetHashCode();
                    HttpContext.Session.SetInt32("CartId", tempCartId);
                }
            }
            return (int)HttpContext.Session.GetInt32("CartId");

        }

        public void AddToCart(int productId)
        {
            var product = _uow.productRepo.getById(productId);
            if (product == null)
            {
                return;
            }
            int cartId = getCartId();
            _uow.cartItemRepo.AddToCart(cartId, productId);
            _uow.SaveChanges();
        }
        public void RemoveFromCart(int productId)
        {
            var product = _uow.productRepo.getById(productId);
            if (product == null)
            {
                return;
            }
            int cartId = getCartId();
            _uow.cartItemRepo.RemoveFromCart(cartId, productId);
            _uow.SaveChanges();
        }

        public void Remove(int productId)
        {
            var product = _uow.productRepo.getById(productId);
            if (product == null)
            {
                return;
            }
            int cartId = getCartId();
            _uow.cartItemRepo.RemoveFromCart(cartId, productId);
            _uow.SaveChanges();
        }

        public void EmptyCart()
        {
            int cartId = getCartId();
            _uow.cartItemRepo.EmptyCart(cartId);
            _uow.SaveChanges();
        }

        public IActionResult ViewCart()
        {
            int cartId = getCartId();
            return View(_uow.cartItemRepo.ViewCart(cartId));
        }

        public double CalculateTotal()
        {
            int cartId = getCartId();
            var cartItems = _uow.cartItemRepo.ViewCart(cartId);
            double total = 0.0;
            foreach (var item in cartItems)
            {
                total += (item.Quantity * item.Product.Price);
            }
            return total;
        }

        [Authorize]
        public IActionResult Checkout()
        {
            CheckoutViewModel cvm = new CheckoutViewModel();
            cvm.total = CalculateTotal();
            return View(cvm);
        }
    }
}
