using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.Repositories.IRepos;
using Microsoft.EntityFrameworkCore;

namespace fullstackecommercewebapp.Repositories.Repos
{
    public class OrderRepo : BaseRepo<Models.Order>, IOrderRepo
    {
        public OrderRepo(AppDbContext db) : base(db)
        {


        }
        public Order getWithDetails(int id)
        {
            return _db.Order.Include(o => o.ProductOrder).Include(o => o.Shipping).Include(o => o.Customer).Where(o => o.Id == id).FirstOrDefault();
        }

        public IEnumerable<Order> getUserOrders(int id)
        {
            return _db.Order.Include(o => o.ProductOrder).Include(o => o.Shipping).Include(o => o.Customer).Where(o => o.CustomerId == id);
        }

        public IEnumerable<Order> getAllWithUsers()
        {
            return _db.Order.Include(o => o.Customer);
        }

        public override void Delete(int id)
        {
            var items = _db.ProductOrder.Where(po => po.OrderId == id);
            foreach (var item in items)
            {
                _db.ProductOrder.Remove(item);
            }
            var shipping = _db.Shipping.Where(s => s.OrderId == id).FirstOrDefault();
            _db.Shipping.Remove(shipping);
            base.Delete(id);
        }
    }
}
