using fullstackecommercewebapp.Repositories.Repos;

namespace fullstackecommercewebapp.UoW
{
    public interface IUoW
    {
        public ProductRepo productRepo { get; set; }
        public AttributeRepo attributeRepo { get; set; }
        public CategoryRepo categoryRepo { get; set; }
        public ProductAttributeValueRepo productAttributeValueRepo { get; set; }
        public RoleRepo roleRepo { get; set; }
        public UserRepo userRepo { get; set; }
        public CategoryAttributeRepo categoryAttributeRepo { get; set; }
        public CartItemRepo cartItemRepo { get; set; }
        public ShippingRepo shippingRepo { get; set; }
        public OrderRepo orderRepo { get; set; }
        public void SaveChanges();
        public void Rollback();
    }
}
