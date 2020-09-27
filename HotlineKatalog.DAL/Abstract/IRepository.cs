using System;
using System.Linq;
using System.Linq.Expressions;

namespace HotlineKatalog.DAL.Abstract
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IQueryable<T> Table { get; }

        IQueryable<T> Get(Expression<Func<T, bool>> predicate);

        T Find(Expression<Func<T, bool>> predicate);

        T GetById(object id);

        void Insert(T entity);

        void UpdateWhere(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> newEntety);

        void Delete(T entity);

        void DeleteById(int id);

        void DeleteWhere(Expression<Func<T, bool>> predicate);

        bool Any(Expression<Func<T, bool>> predicate);

        int Count(Expression<Func<T, bool>> predicate);
    }
}
