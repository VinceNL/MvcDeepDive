using MvcShop.Infrastructure.Data;

namespace MvcShop.Infrastructure.Repositories
{
    public abstract class GenericRepository<T>
        : IRepository<T> where T : class
    {
        protected MvcShopContext _context;

        public GenericRepository(MvcShopContext context)
        {
            _context = context;
        }

        public virtual T Add(T entity)
        {
            var addedEntity = _context.Add(entity).Entity;
            return addedEntity;
        }

        public T? Get(Guid id)
        {
            return _context.Find<T>(id);
        }

        public IEnumerable<T> GetAll()
        {
            var all = _context.Set<T>().ToList();
            return all;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public T Update(T entity)
        {
            return _context.Update(entity).Entity;
        }
    }
}