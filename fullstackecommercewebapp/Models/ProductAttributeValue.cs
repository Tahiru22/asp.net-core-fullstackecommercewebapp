namespace fullstackecommercewebapp.Models
{
    public partial class ProductAttributeValue : BaseEntity
    {
        public int ProductId { get; set; }
        public int AttributeId { get; set; }
        public int Id { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }

        public virtual Attributes Attribute { get; set; }
        public virtual Product Product { get; set; }
    }
}
