using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NPatterns.ObjectRelational.EntityFramework
{
    /// <summary>
    ///     implement the IRepository with EntityFramework
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
        protected DbSet<TEntity> Set { get; private set; }

        #region IRepository<T> Members

        public IQueryable<TEntity> Query()
        {
            return Set;
        }

        public TEntity Find(params object[] keyValues)
        {
            return Set.Find(keyValues);
        }

        public Task<TEntity> FindAsync(params object[] keyValues)
        {
            return Set.FindAsync(keyValues);
        }

        public Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return Set.FindAsync(cancellationToken, keyValues);
        }

        public void Add(TEntity entity)
        {
            Set.Add(entity);
        }

        public void Modify(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
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