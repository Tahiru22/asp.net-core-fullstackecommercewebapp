using fullstackecommercewebapp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace fullstackecommercewebapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackController : BaseController
    {
        public TrackController()
        {
        }


        [HttpGet("{id}")]
        public TrackViewModel TrackOrder(int id)
        {
            var order = _uow.orderRepo.getById(id);
            TrackViewModel tvm = new TrackViewModel();
            tvm.Number = id;
            if (order == null)
            {
                tvm.Status = "There is No Requested Order With This Number";
            }
            else
            {
                tvm.Status = order.Status;
            }
            return tvm;
        }

    }
}
