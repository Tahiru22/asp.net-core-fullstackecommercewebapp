namespace fullstackecommercewebapp.Models
{
    public partial class SaleProduct : BaseEntity
    {
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int Id { get; set; }
        public double Discount { get; set; }

        public virtual Product Product { get; set; }
        public virtual Sale Sale { get; set; }
    }
}
