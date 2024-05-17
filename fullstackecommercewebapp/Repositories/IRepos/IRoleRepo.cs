using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Models;

namespace fullstackecommercewebapp.Repositories.IRepos
{
    public interface IRoleRepo : IBaseRepo<AspNetRoles>
    {
        public int checkUnique(string Name);
        public int checkDelete(int id);
    }
}
