namespace fullstackecommercewebapp.Models
{
    public partial class Coupon : BaseEntity
    {
        public Coupon()
        {
            CouponProduct = new HashSet<CouponProduct>();
            UserHasCoupon = new HashSet<UserHasCoupon>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<CouponProduct> CouponProduct { get; set; }
        public virtual ICollection<UserHasCoupon> UserHasCoupon { get; set; }
    }
}
