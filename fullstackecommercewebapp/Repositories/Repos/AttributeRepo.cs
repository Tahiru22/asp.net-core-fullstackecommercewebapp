using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Repositories.IRepos;

namespace fullstackecommercewebapp.Repositories.Repos
{
    public class AttributeRepo : BaseRepo<Models.Attributes>, IAttributeRepo
    {
        public AttributeRepo(AppDbContext db) : base(db)
        {
            
        }
        public int checkUnique(string Name)
        {
            int id = _db.Attribute.Where(p => p.Name == Name).Select(p => p.Id).FirstOrDefault();
            return id;
        }
        public int checkDelete(int id)
        {
            return _db.ProductAttributeValue.Where(pav => pav.AttributeId == id).Select(pav => pav.Id).Count()
                + _db.CategoryAttribute.Where(pav => pav.AttributeId == id).Select(pav => pav.Id).Count();
        }
    }
}
