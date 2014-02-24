using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NPatterns.ObjectRelational.EntityFramework
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
                         select (IAuditable) e.Entity).ToList();

            toAdd.ForEach(z =>
            {
                z.Created = DateTime.Now;
                z.CreatedBy = Thread.CurrentPrincipal.Identity.Name;
            });

            var toUpdate = (from e in Context.ChangeTracker.Entries()
                            where e.State == EntityState.Modified && e.Entity is IAuditable
                            select (IAuditable) e.Entity).ToList();

            toUpdate.ForEach(z =>
            {
                z.Updated = DateTime.Now;
                z.UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
            });

            var toDel = (from e in Context.ChangeTracker.Entries()
                         where e.State == EntityState.Deleted && e.Entity is IArchivable
                         select e).ToList();

            toDel.ForEach(z =>
            {
                z.State = EntityState.Modified;
                ((IArchivable) z.Entity).Deleted = DateTime.Now;
                ((IArchivable) z.Entity).DeletedBy = Thread.CurrentPrincipal.Identity.Name;
            });
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