using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.Services;
using fullstackecommercewebapp.ViewModels;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PagedList;
using PayStack.Net;
using System.Text;

namespace fullstackecommercewebapp.Controllers
{
    public class OrderController : BaseController
    {
        public static string msg = "";
        public static int id = 0;
        private readonly string token;
        private readonly IConfiguration _configuration;
        private PayStackApi Paystack { get; set; }
        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
            token = _configuration["Payment:PaystackSK"];
            Paystack = new PayStackApi(token);
        }

        [Authorize]
        public IActionResult getMyOrders(string sortOrder, string OrderSearchString, string CurrentOrderFilter, int? page)
        {
            ViewBag.msg = msg;
            ViewBag.OrderId = id;
            id = 0;
            msg = "";
            ViewBag.CurrentSort = sortOrder;
            int user_id = _uow.userRepo.getByUserName(User.Identity.Name).Id;
            var orders = _uow.orderRepo.getUserOrders(user_id);
            ViewBag.NumberSortParm = String.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (OrderSearchString != null)
            {
                page = 1;
            }
            else
            {
                OrderSearchString = CurrentOrderFilter;
            }
            ViewBag.CurrentOrderFilter = OrderSearchString;

            if (!String.IsNullOrEmpty(OrderSearchString))
            {
                orders = orders.Where(o => o.Id.ToString().Contains(OrderSearchString));
            }
            switch (sortOrder)
            {
                case "Date":
                    orders = orders.OrderBy(s => s.CreatedAt);
                    break;
                case "date_desc":
                    orders = orders.OrderByDescending(s => s.CreatedAt);
                    break;
                case "number_desc":
                    orders = orders.OrderByDescending(s => s.Id);
                    break;
                default:
                    orders = orders.OrderBy(s => s.Id);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(orders.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Administrator, Delievery Employee")]
        public IActionResult Index(string sortOrder, string CustomerSearchString, string OrderSearchString,
            string CurrentCustomerFilter, string CurrentOrderFilter, int? page)
        {
            ViewBag.msg = msg;
            msg = "";
            var orders = _uow.orderRepo.getAllWithUsers();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NumberSortParm = String.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (CustomerSearchString != null || OrderSearchString != null)
            {
                page = 1;
            }
            else
            {
                CustomerSearchString = CurrentCustomerFilter;
                OrderSearchString = CurrentOrderFilter;
            }
            ViewBag.CurrentCustomerFilter = CustomerSearchString;
            ViewBag.CurrentOrderFilter = OrderSearchString;

            if (!String.IsNullOrEmpty(OrderSearchString))
            {
                orders = orders.Where(o => o.Id.ToString().Contains(OrderSearchString));
            }
            if (!String.IsNullOrEmpty(CustomerSearchString))
            {
                orders = orders.Where(o => o.Customer.FirstName.ToLower().Contains(CustomerSearchString.ToLower())
                || o.Customer.LastName.ToLower().Contains(CustomerSearchString.ToLower()));
            }
            switch (sortOrder)
            {
                case "Name":
                    orders = orders.OrderByDescending((s => s.Customer.FirstName + " " + s.Customer.LastName));
                    break;
                case "name_desc":
                    orders = orders.OrderByDescending((s => s.Customer.FirstName + " " + s.Customer.LastName));
                    break;
                case "Date":
                    orders = orders.OrderBy(s => s.CreatedAt);
                    break;
                case "date_desc":
                    orders = orders.OrderByDescending(s => s.CreatedAt);
                    break;
                case "number_desc":
                    orders = orders.OrderByDescending(s => s.Id);
                    break;
                default:
                    orders = orders.OrderBy(s => s.Id);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(orders.ToPagedList(pageNumber, pageSize));
        }

     

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add(CheckoutViewModel cvm)
        {
            if (ModelState.IsValid && HttpContext.Session.GetInt32("CartId") != null)
            {
                var user = _uow.userRepo.getByUserName(User.Identity.Name);
                var callbackUrl = Url.Action(nameof(VerifyPayment), "Order", null, Request.Scheme);

                var paystackService = new PaystackService(new HttpClient(), _configuration);

                // Convert Total to decimal before passing it to InitializeTransactionAsync
                var totalAmount = Convert.ToDecimal(cvm.total);

                var authorizationUrl = await paystackService.InitializeTransactionAsync(totalAmount, user.Email, callbackUrl);

                // Store necessary information in TempData or session to retrieve after payment verification
                TempData["CartId"] = HttpContext.Session.GetInt32("CartId");
                TempData["UserId"] = user.Id;
                TempData["Address"] = string.IsNullOrEmpty(cvm.Address) ? user.Address : cvm.Address;
                TempData["Phone"] = string.IsNullOrEmpty(cvm.Phone) ? user.Phone : cvm.Phone;

                // Convert Total to string before storing it in TempData
                TempData["Total"] = cvm.total.ToString();

                return Redirect(authorizationUrl);
            }
            else
            {
                ModelState.AddModelError("", "Unable to resolve your Request. Try again, and if the problem persists see your system administrator.");
                return View("Views/Shop/Checkout", cvm);
            }
        }

        [Authorize]
        public async Task<IActionResult> VerifyPayment(string reference)
        {
            if (string.IsNullOrEmpty(reference))
            {
                // Handle error (invalid reference)
                return RedirectToAction("Error", "Home");
            }

            var paystackService = new PaystackService(new HttpClient(), _configuration);
            var verificationResponse = await paystackService.VerifyTransaction(reference);

            if (verificationResponse.Status && verificationResponse.Data.Status == "success")
            {
                // Transaction was successful
                var userId = (int)TempData["UserId"];
                var cartId = (int)TempData["CartId"];
                var address = TempData["Address"].ToString();
                var phone = TempData["Phone"].ToString();

                // Parse Total back to double
                var total = double.Parse(TempData["Total"].ToString());

                var items = _uow.cartItemRepo.ViewCart(cartId);
                Order order = new Order()
                {
                    Status = "In process",
                    CustomerId = userId,
                    Total = total
                };
                foreach (var item in items)
                {
                    order.ProductOrder.Add(new ProductOrder()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                    });
                }
                order.Shipping = new Shipping()
                {
                    Address = address,
                    Phone = phone,
                    Order = order
                };

                foreach (var item in items)
                {
                    _uow.cartItemRepo.Delete(item.Id);
                }
                _uow.orderRepo.Add(order);
                _uow.SaveChanges();

                // Redirect to order confirmation page
                return RedirectToAction("OrderConfirmation", new { id = order.Id });
            }
            else
            {
                // Handle failed transaction
                return RedirectToAction("PaymentFailed");
            }
        }


       
        public IActionResult OrderConfirmation(int id)
        {
            var order = _uow.orderRepo.getById(id);
            return View(order);
        }

        public IActionResult PaymentFailed()
        {
            return View();
        }





        [Authorize(Roles = "Administrator, Delievery Employee")]
        [HttpGet]
        public IActionResult updateOrderStatus(int id)
        {
            var order = _uow.orderRepo.getWithDetails(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var AllProducts = _uow.productRepo.getAll();
            foreach (var product in AllProducts)
            {
                ViewData[(product.Id).ToString()] = product.Name;
            }
            return View(order);
        }

        [Authorize(Roles = "Administrator, Delievery Employee")]
        [HttpPost]
        public IActionResult updateOrderStatus()
        {
            int id = Convert.ToInt32(Request.Form["id"]);
            string status = Request.Form["status"];
            var order = _uow.orderRepo.getById(id);
            order.Status = status;
            _uow.orderRepo.Edit(order);
            _uow.SaveChanges();
            msg = "statusEdited";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator, Delievery Employee")]
        public IActionResult Details(int id)
        {
            var order = _uow.orderRepo.getWithDetails(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var AllProducts = _uow.productRepo.getAll();
            foreach (var product in AllProducts)
            {
                ViewData[(product.Id).ToString()] = product.Name;
            }
            return View(order);
        }

        [Authorize]
        public IActionResult MyOrderDetails(int id)
        {
            var order = _uow.orderRepo.getWithDetails(id);
            if (order == null)
            {
                return RedirectToAction("getMyOrders");
            }
            var AllProducts = _uow.productRepo.getAll();
            foreach (var product in AllProducts)
            {
                ViewData[(product.Id).ToString()] = product.Name;
            }
            return View("Details", order);
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            var order = _uow.orderRepo.getById(id);
            if (order == null)
            {
                return RedirectToAction("getMyOrders");
            }
            if (order.Status == "In Process")
            {
                _uow.orderRepo.Delete(id);
                msg = "deleted";
            }
            else
            {
                msg = "NoDelete";
            }
            _uow.SaveChanges();
            return RedirectToAction("getMyOrders");
        }
    }
}
