using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Repositories.IRepos;

namespace fullstackecommercewebapp.Repositories.Repos
{
    public class RoleRepo: BaseRepo<Models.AspNetRoles>, IRoleRepo
    {
        public RoleRepo(AppDbContext db) : base(db)
        {


        }
        public int checkUnique(string Name)
        {
            int id = _db.Role.Where(p => p.Name == Name).Select(p => p.Id).FirstOrDefault();
            return id;
        }
        public int checkDelete(int id)
        {
            return _db.UserRoles.Where(ca => ca.RoleId == id).Select(pav => pav.RoleId).Count()
                + _db.RoleClaims.Where(p => p.RoleId == id).Select(p => p.Id).Count();
        }
    }
}
