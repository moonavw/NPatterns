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
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Convert to IQueryable for querying
        /// </summary>
        /// <returns>IQueryable for entity</returns>
        IQueryable<TEntity> AsQueryable();

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

    public static class RepositoryExtensions
    {
        public static IQueryable<T> FindAll<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate) where T : class
        {
            return repository.AsQueryable().Where(predicate);
        }

        public static T Find<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate) where T : class
        {
            return FindAll(repository, predicate).FirstOrDefault();
        }
    }
}