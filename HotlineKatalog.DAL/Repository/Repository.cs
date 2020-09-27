using HotlineKatalog.DAL.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace HotlineKatalog.DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private IDataContext _context = null;
        private DbSet<T> _entities;

        public Repository(IDataContext context)
        {
            _context = context;
        }

        private DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();

                return _entities;
            }
        }

        private void ThrowIfEntityIsNull(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return Entities;
            }
        }

        public IList<T> GetAll()
        {
            return Entities.ToList();
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return Entities.Where(predicate);
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            return Entities.FirstOrDefault(predicate);
        }

        public T GetById(object id)
        {
            return Entities.Find(id);
        }

        public void Insert(T entity)
        {
            try
            {
                ThrowIfEntityIsNull(entity);

                Entities.Add(entity);
            }
            catch (Exception dbEx)
            {
                throw;
            }
        }

        public void UpdateWhere(Expression<Func<T, bool>> predicate, Expression<Func<T, T>> newEntety)
        {
            try
            {
                Entities.Where(predicate).UpdateFromQuery(newEntety);
            }
            catch (Exception dbEx)
            {
                throw dbEx;
            }
        }

        public void Delete(T entity)
        {
            try
            {
                ThrowIfEntityIsNull(entity);

                Entities.Remove(entity);
            }
            catch (Exception dbEx)
            {
                throw dbEx;
            }
        }

        public void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            try
            {
                Entities.Where(predicate).Delete();
            }
            catch (Exception dbEx)
            {
                throw dbEx;
            }
        }

        public void DeleteById(int id)
        {
            try
            {
                T entity = GetById(id);

                ThrowIfEntityIsNull(entity);

                Entities.Remove(entity);
            }
            catch (Exception dbEx)
            {
                throw;
            }
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return Entities.Any(predicate);
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return Entities.Count(predicate);
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).                  
                    _context.Dispose();
                    _entities = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Repository()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
