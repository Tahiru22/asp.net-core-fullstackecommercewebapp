namespace fullstackecommercewebapp.Models
{
    public partial class CategoryAttribute : BaseEntity
    {
        public int CategoryId { get; set; }
        public int AttributeId { get; set; }
        public int Id { get; set; }

        public virtual Attributes Attribute { get; set; }
        public virtual Category Category { get; set; }
    }
}
