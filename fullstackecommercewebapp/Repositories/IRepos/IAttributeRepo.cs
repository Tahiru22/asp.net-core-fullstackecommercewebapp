namespace fullstackecommercewebapp.Repositories.IRepos
{
    public interface IAttributeRepo: IBaseRepo<Models.Attributes>
    {
        public int checkUnique(string Name);
        public int checkDelete(int id);
    }
}
