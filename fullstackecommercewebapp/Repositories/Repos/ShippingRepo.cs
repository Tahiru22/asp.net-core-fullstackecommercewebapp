using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.Repositories.IRepos;

namespace fullstackecommercewebapp.Repositories.Repos
{
    public class ShippingRepo: BaseRepo<Shipping>, IShippingRepo
    {
        public ShippingRepo(AppDbContext db) : base(db)
        {


        }
    }
}
