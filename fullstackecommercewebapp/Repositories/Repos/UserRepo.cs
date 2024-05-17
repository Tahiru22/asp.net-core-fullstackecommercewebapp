using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.Repositories.IRepos;

namespace fullstackecommercewebapp.Repositories.Repos
{
    public class UserRepo: BaseRepo<User>, IUserRepo
    {
        public UserRepo(AppDbContext db) : base(db)
        {
        }
        public int checkDelete(int id)
        {
            return _db.CartItem.Where(ca => ca.CartId == id).Select(pav => pav.Id).Count()
                + _db.Order.Where(p => p.CustomerId == id).Select(p => p.Id).Count();
        }
        public int checkEmailUnique(string Email)
        {
            return _db.User.Where(ca => ca.Email == Email).Select(pav => pav.Id).FirstOrDefault();
        }

        public int checkUserNameUnique(string UserName)
        {
            return _db.User.Where(ca => ca.UserName == UserName).Select(pav => pav.Id).FirstOrDefault();
        }
        public int checkPhoneUnique(string phone)
        {
            return _db.User.Where(ca => ca.Phone == phone).Select(pav => pav.Id).FirstOrDefault();
        }
        public User getByUserName(string userName)
        {
            return _db.User.Where(ca => ca.UserName == userName).FirstOrDefault();
        }
    }
}
