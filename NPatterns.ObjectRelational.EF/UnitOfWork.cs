using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NPatterns.ObjectRelational.EF
{
    /// <summary>
    /// implement the IUnitOfWork with EntityFramework
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(DbContext context)
        {
            Context = context;
        }

        protected DbContext Context { get; private set; }

        #region IUnitOfWork Members

        public void Commit()
        {
            OnCommitting();
            Context.SaveChanges();
        }

        public Task CommitAsync()
        {
            OnCommitting();
            return Context.SaveChangesAsync();
        }

        public void CommitAndRefresh()
        {
            bool saveFailed;

            do
            {
                try
                {
                    Commit();

                    saveFailed = false;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    ex.Entries.ToList()
                      .ForEach(entry => entry.OriginalValues.SetValues(entry.GetDatabaseValues()));
                }
            } while (saveFailed);
        }

        public void Rollback()
        {
            // set all entities in change tracker 
            // as 'unchanged state'
            Context.ChangeTracker.Entries()
                   .ToList()
                   .ForEach(entry => entry.State = EntityState.Unchanged);
        }

        protected virtual void OnCommitting()
        {
            var toAdd = (from e in Context.ChangeTracker.Entries()
                         where e.State == EntityState.Added && e.Entity is IAuditable
                         select e).ToList();

            toAdd.ForEach(z =>
            {
                ((IAuditable)z.Entity).Created = DateTime.Now;
                ((IAuditable)z.Entity).CreatedBy = Thread.CurrentPrincipal.Identity.Name;

                //readonly for updated audit
                ((IAuditable)z.Entity).Updated = null;
                ((IAuditable)z.Entity).UpdatedBy = null;
            });

            var toUpdate = (from e in Context.ChangeTracker.Entries()
                            where e.State == EntityState.Modified && e.Entity is IAuditable
                            select e).ToList();

            toUpdate.ForEach(z =>
            {
                //readonly for created audit
                z.Property("Created").IsModified = false;
                z.Property("CreatedBy").IsModified = false;

                ((IAuditable)z.Entity).Updated = DateTime.Now;
                ((IAuditable)z.Entity).UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
            });

            var toDel = (from e in Context.ChangeTracker.Entries()
                         where e.State == EntityState.Deleted && e.Entity is IArchivable
                         select e).ToList();

            toDel.ForEach(z =>
            {
                z.State = EntityState.Modified;
                ((IArchivable)z.Entity).Archive();
                if (z.Entity is IAuditable)
                {
                    //readonly for audit
                    z.Property("Created").IsModified = false;
                    z.Property("CreatedBy").IsModified = false;
                    z.Property("Updated").IsModified = false;
                    z.Property("UpdatedBy").IsModified = false;
                }
            });
        }

        public void MarkNew<TEntity>(TEntity entity) where TEntity : class
        {
            Context.Entry(entity).State = EntityState.Added;
        }

        public void MarkUnchanged<TEntity>(TEntity entity) where TEntity : class
        {
            Context.Entry(entity).State = EntityState.Unchanged;
        }

        public void MarkModified<TEntity>(TEntity entity) where TEntity : class
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void MarkDeleted<TEntity>(TEntity entity) where TEntity : class
        {
            Context.Entry(entity).State = EntityState.Deleted;
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