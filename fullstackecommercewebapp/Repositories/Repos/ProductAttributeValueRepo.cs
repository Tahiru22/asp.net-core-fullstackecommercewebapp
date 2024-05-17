using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.Repositories.IRepos;

namespace fullstackecommercewebapp.Repositories.Repos
{
    public class ProductAttributeValueRepo : BaseRepo<ProductAttributeValue>, IProductAttributeValue
    {
        public ProductAttributeValueRepo(AppDbContext db) : base(db)
        {

        }
    }
}
