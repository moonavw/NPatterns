using System;
using System.Linq;
using System.Linq.Expressions;

namespace NPatterns.ObjectRelational
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> AsQueryable();
        T Find(params object[] keyValues);
        void Add(T entity);
        void Attach(T entity);
        void Remove(T entity);
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