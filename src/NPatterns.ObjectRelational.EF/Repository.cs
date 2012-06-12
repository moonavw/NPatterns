using System.Data.Entity;
using System.Linq;

namespace NPatterns.ObjectRelational.EF
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IDbSet<T> _dbSet;

        public Repository(IDbSet<T> dbSet)
        {
            _dbSet = dbSet;
        }

        #region IRepository<T> Members

        public IQueryable<T> AsQueryable()
        {
            return _dbSet; //.AsNoTracking();
        }

        public T Find(params object[] keyValues)
        {
            return _dbSet.Find(keyValues);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Attach(T entity)
        {
            _dbSet.Attach(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        #endregion
    }
}