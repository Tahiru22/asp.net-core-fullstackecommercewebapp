using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.Repositories.IRepos;

namespace fullstackecommercewebapp.Repositories.Repos
{
    public class CategoryAttributeRepo : BaseRepo<CategoryAttribute>, ICategoryAttributeRepo
    {
        public CategoryAttributeRepo(AppDbContext db) : base(db)
        {

        }

    }
}
