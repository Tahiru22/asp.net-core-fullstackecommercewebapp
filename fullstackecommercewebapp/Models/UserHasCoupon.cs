namespace fullstackecommercewebapp.Models
{

    public partial class UserHasCoupon : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CouponId { get; set; }

        public virtual Coupon Coupon { get; set; }
        public virtual User User { get; set; }
    }
}
