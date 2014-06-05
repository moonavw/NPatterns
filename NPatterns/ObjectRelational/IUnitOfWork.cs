using System;
using System.Threading.Tasks;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// Contract for "UnitOfWork pattern". For more
    /// related info see http://martinfowler.com/eaaCatalog/unitOfWork.html or
    /// http://msdn.microsoft.com/en-us/magazine/dd882510.aspx
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commit all changes made in a container.
        /// </summary>
        /// <remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,
        /// then an exception is thrown
        /// </remarks>
        void Commit();

        /// <summary>
        /// async Commit all changes made in a container.
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// Commit all changes made in a container.
        /// </summary>
        /// <remarks>
        /// If the entity have fixed properties and any optimistic concurrency problem exists,
        /// then 'client changes' are refreshed - Client wins
        /// </remarks>
        void CommitAndRefresh();

        /// <summary>
        /// Rollback tracked changes. See references of UnitOfWork pattern
        /// </summary>
        void Rollback();

        /// <summary>
        /// Mark entity as new Added
        /// </summary>
        void MarkNew<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Mark entity as Unchanged
        /// </summary>
        void MarkUnchanged<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Mark entity as Modified
        /// </summary>
        void MarkModified<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Mark entity as Deleted
        /// </summary>
        void MarkDeleted<TEntity>(TEntity entity) where TEntity : class;
    }
}