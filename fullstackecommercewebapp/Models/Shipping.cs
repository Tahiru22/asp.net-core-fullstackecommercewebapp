namespace fullstackecommercewebapp.Models
{
    public partial class Shipping : BaseEntity
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
