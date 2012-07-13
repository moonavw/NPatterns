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

        public void Add(T entity)
        {
            Set.Add(entity);
        }

        public void Modify(T entity)
        {
            Context.Entry(entity).State = System.Data.EntityState.Modified;
        }

        public void Track(T entity)
        {
            Set.Attach(entity);
        }

        public void Merge(T original, T current)
        {
            Context.Entry(original).CurrentValues.SetValues(current);
        }

        public void Remove(T entity)
        {
            Set.Remove(entity);
        }

        #endregion
    }
}