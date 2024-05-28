using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Repositories.IRepos;
using Microsoft.EntityFrameworkCore;

namespace fullstackecommercewebapp.Repositories.Repos
{
    public class BaseRepo <T> : IBaseRepo<T> where T : class
    {
        protected DbSet<T> _dbSet;
        protected AppDbContext _db;
        public BaseRepo(AppDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

        public IEnumerable<T> getAll()
        {
             return _dbSet.ToList();
        }
        public T getById(int id)
        {
            return _dbSet.Find(id);
        }
        public void Add(T obj)
        {
            _dbSet.Add(obj);
        }
        public void Edit(T obj)
        {
            _dbSet.Update(obj);
        }
        public virtual void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            _dbSet.Remove(entity);
        }
    }
}
