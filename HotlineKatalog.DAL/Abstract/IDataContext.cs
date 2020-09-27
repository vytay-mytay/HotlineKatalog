using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace HotlineKatalog.DAL.Abstract
{
    public interface IDataContext : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbQuery<TEntity> Query<TEntity>() where TEntity : class;

        IModel Model { get; }

        int SaveChanges();
    }
}
