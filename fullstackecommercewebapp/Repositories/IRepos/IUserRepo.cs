using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Models;

namespace fullstackecommercewebapp.Repositories.IRepos
{
    public interface IUserRepo: IBaseRepo<Models.User>
    {
        public int checkDelete(int id);

        public int checkEmailUnique(string Email);
        public int checkUserNameUnique(string UserName);
        public int checkPhoneUnique(string PhoneNumber);
        public User getByUserName(string userName);
    }
}
