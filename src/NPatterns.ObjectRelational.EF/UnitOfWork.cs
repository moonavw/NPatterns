using System.Data;
using System.Data.Entity;

namespace NPatterns.ObjectRelational.EF
{
    public class UnitOfWork : DbContext, IUnitOfWork
    {
        #region IUnitOfWork Members

        public void Commit()
        {
            SaveChanges();
        }

        public IRepository<T> Repository<T>() where T : class
        {
            return new Repository<T>(Set<T>());
        }

        public void MarkNew<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Added;
        }

        public void MarkModified<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Modified;
        }

        public void MarkDeleted<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Deleted;
        }

        public void MarkUnchanged<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Unchanged;
        }

        public void MarkDetached<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Detached;
        }

        #endregion
    }
}