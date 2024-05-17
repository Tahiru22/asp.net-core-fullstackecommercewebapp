using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace fullstackecommercewebapp.Controllers
{
    public class OrderController : BaseController
    {
        public static string msg = "";
        public static int id = 0;

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
        public IActionResult Add(CheckoutViewModel cvm)
        {
            if (ModelState.IsValid && HttpContext.Session.GetInt32("CartId") != null)
            {
                var user = _uow.userRepo.getByUserName(User.Identity.Name);
                int user_id = user.Id;
                Order order = new Order()
                {
                    Status = "In Process",
                    CustomerId = user_id,
                    Total = cvm.total
                };
                var items = _uow.cartItemRepo.ViewCart(user_id);
                foreach (var item in items)
                {
                    order.ProductOrder.Add(new ProductOrder()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                    });
                }
                var Address = user.Address;
                var Phone = user.Phone;
                if (!string.IsNullOrEmpty(cvm.Address))
                {
                    Address = cvm.Address;
                }
                if (!string.IsNullOrEmpty(cvm.Phone))
                {
                    Phone = cvm.Phone;
                }
                order.Shipping = new Shipping()
                {
                    Address = Address,
                    Phone = Phone,
                    Order = order
                };
                foreach (var item in items)
                {
                    _uow.cartItemRepo.Delete(item.Id);
                }
                _uow.orderRepo.Add(order);
                _uow.SaveChanges();
                id = order.Id;
            }
            else
            {
                ModelState.AddModelError("", "Unable to resolve your Request. Try again, and if the problem persists see your system administrator.");
                return View("Views/Shop/Checkout.cshtml", cvm);
            }
            msg = "added";
            return RedirectToAction("getMyOrders");
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
