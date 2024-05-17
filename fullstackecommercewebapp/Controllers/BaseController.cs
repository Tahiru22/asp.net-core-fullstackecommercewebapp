using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.UoW;
using Microsoft.AspNetCore.Mvc;

namespace fullstackecommercewebapp.Controllers
{
    public class BaseController : Controller
    {
        protected IUoW _uow;

        public BaseController()
        {
            _uow = new fullstackecommercewebapp.UoW.UoW(new fullstackecommercewebapp.Data.AppDbContext());

        }
    }
}
