using System;
using System.Linq;
using System.Linq.Expressions;

namespace NPatterns.ObjectRelational
{
    /// <summary>
    /// Base interface for implement a "Repository Pattern", for
    /// more information about this pattern see http://martinfowler.com/eaaCatalog/repository.html
    /// </summary>
    /// <typeparam name="TEntity">Type of entity for this repository </typeparam>
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        /// <summary>
        /// Convert to IQueryable for querying
        /// </summary>
        /// <returns>IQueryable for entity</returns>
        IQueryable<TEntity> AsQueryable();

        /// <summary>
        /// Find all elements by predicate
        /// </summary>
        /// <param name="predicate">expression of predicate</param>
        /// <returns>queryable result</returns>
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Find single(first or default) element by predicate
        /// </summary>
        /// <param name="predicate">expression of predicate</param>
        /// <returns>entity</returns>
        TEntity Find(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Add entity into repository
        /// </summary>
        /// <param name="entity">Item to add</param>
        void Add(TEntity entity);

        /// <summary>
        /// Delete entity from repository
        /// </summary>
        /// <param name="entity">Item to delete</param>
        void Remove(TEntity entity);

        /// <summary>
        /// Mark entity as modified
        /// </summary>
        /// <param name="entity">Item to modify</param>
        void Modify(TEntity entity);

        /// <summary>
        /// Track entity into this repository
        /// </summary>
        /// <param name="entity">Item to track</param>
        void Track(TEntity entity);

        /// <summary>
        /// Sets modified entity into the repository. 
        /// When calling Commit() method in UnitOfWork 
        /// these changes will be saved into the storage
        /// </summary>
        /// <param name="original">The persisted item</param>
        /// <param name="current">The current item</param>
        void Merge(TEntity original, TEntity current);
    }
}