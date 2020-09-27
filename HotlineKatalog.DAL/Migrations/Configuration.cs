using HotlineKatalog.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotlineKatalog.DAL.Migrations
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext context, IConfiguration Configuration, IServiceProvider serviceProvider)
        {
            context.Database.EnsureCreated();

            #region Categories

            new List<Category>
            {
                new Category { Name = "Холодильник" }
            }
            .ForEach(i =>
            {
                if (!context.Categories.Any(x => x.Name == i.Name))
                    context.Categories.Add(i);
            });

            context.SaveChanges();

            #endregion
        }
    }
}
