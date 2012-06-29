using System.Data.Entity;

namespace NPatterns.ObjectRelational.EF
{
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

        IRepository<T> IUnitOfWork.Repository<T>()
        {
            return new Repository<T>(Context);
        }

        #endregion
    }
}