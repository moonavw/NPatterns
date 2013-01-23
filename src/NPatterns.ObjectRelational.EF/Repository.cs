using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace NPatterns.ObjectRelational.EF
{
    /// <summary>
    /// implement the IRepository with EF
    /// </summary>
    /// <typeparam name="TEntity">type of the entity in this repository</typeparam>
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        public Repository(DbContext context)
        {
            Context = context;
            Set = Context.Set<TEntity>();
        }

        protected DbContext Context { get; private set; }
        protected IDbSet<TEntity> Set { get; private set; }

        #region IRepository<T> Members

        public IQueryable<TEntity> AsQueryable()
        {
            return Set;
        }

        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.Where(predicate);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Set.FirstOrDefault(predicate);
        }

        public void Add(TEntity entity)
        {
            Set.Add(entity);
        }

        public void Modify(TEntity entity)
        {
            Context.Entry(entity).State = System.Data.EntityState.Modified;
        }

        public void Track(TEntity entity)
        {
            Set.Attach(entity);
        }

        public void Merge(TEntity original, TEntity current)
        {
            Context.Entry(original).CurrentValues.SetValues(current);
        }

        public void Remove(TEntity entity)
        {
            Set.Remove(entity);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion
    }
}