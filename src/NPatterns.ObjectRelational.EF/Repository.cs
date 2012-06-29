using System.Data.Entity;
using System.Linq;

namespace NPatterns.ObjectRelational.EF
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public Repository(DbContext context)
        {
            Context = context;
            Set = Context.Set<T>();
        }

        protected DbContext Context { get; private set; }
        protected IDbSet<T> Set { get; private set; }

        #region IRepository<T> Members

        public IQueryable<T> AsQueryable()
        {
            return Set;
        }

        public T Find(params object[] keyValues)
        {
            return Set.Find(keyValues);
        }

        public void Add(T entity)
        {
            Set.Add(entity);
        }

        public void Attach(T entity)
        {
            Set.Attach(entity);
        }

        public void Remove(T entity)
        {
            Set.Remove(entity);
        }

        #endregion
    }
}