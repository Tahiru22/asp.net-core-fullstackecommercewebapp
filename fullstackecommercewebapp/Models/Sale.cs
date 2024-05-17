namespace fullstackecommercewebapp.Models
{

    public partial class Sale : BaseEntity
    {
        public Sale()
        {
            SaleProduct = new HashSet<SaleProduct>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<SaleProduct> SaleProduct { get; set; }
    }
}
