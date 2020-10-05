using HotlineKatalog.DAL.Abstract;
using HotlineKatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace HotlineKatalog.DAL
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.SetCommandTimeout(500);
        }

        public virtual DbSet<Price> Prices { get; set; }
        public virtual DbSet<Good> Goods { get; set; }
        public virtual DbSet<Specification> Specifications { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<ShopCategory> ShopCategories { get; set; }
        public virtual DbSet<ShopTags> ShopTags { get; set; }
        public virtual DbSet<ShopGood> ShopGoods { get; set; }
        public virtual DbSet<Producer> Producers { get; set; }
    }
}
