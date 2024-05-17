using fullstackecommercewebapp.Models;

namespace fullstackecommercewebapp.Repositories.IRepos
{
    public interface ICategoryRepo
    {
        public IEnumerable<Models.Attributes> getCategoryAttributes(int id);
        public IEnumerable<Models.Attributes> getNonCategoryAttributes(int id);
        public IEnumerable<Product> getCategoryProducts(int id);
        public int checkUnique(string Name);
        public int checkDelete(int id);
        public Category getWithAttributeById(int id);
        public void deleteCategoryAttributes(int Id);
    }
}
