using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Models;

namespace fullstackecommercewebapp.Repositories.IRepos
{
    public interface IOrderRepo: IBaseRepo<Models.Order>
    {
        public Order getWithDetails(int id);
        public IEnumerable<Order> getUserOrders(int id);
        public IEnumerable<Order> getAllWithUsers();
    }
}
