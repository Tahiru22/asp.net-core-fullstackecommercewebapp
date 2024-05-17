namespace fullstackecommercewebapp.Models
{
    public partial class CouponProduct : BaseEntity
    {
        public int CouponId { get; set; }
        public int ProductId { get; set; }
        public int Id { get; set; }
        public double Discount { get; set; }

        public virtual Coupon Coupon { get; set; }
        public virtual Product Product { get; set; }
    }
}
