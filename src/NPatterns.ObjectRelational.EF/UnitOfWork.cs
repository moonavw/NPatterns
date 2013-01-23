using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace NPatterns.ObjectRelational.EF
{
    /// <summary>
    /// implement the IUnitOfWork with EF
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
            Context.SaveChanges();
        }

        public void CommitAndRefresh()
        {
            bool saveFailed = false;

            do
            {
                try
                {
                    Context.SaveChanges();

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
                              .ForEach(entry => entry.State = System.Data.EntityState.Unchanged);
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