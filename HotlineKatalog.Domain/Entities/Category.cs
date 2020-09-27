using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotlineKatalog.Domain.Entities
{
    public class Category : IEntity<int>
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty(nameof(Category))]
        public virtual ICollection<Good> Goods { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<ShopCategory> Shops { get; set; }

        #endregion

        #region Ctors

        public Category()
        {
            Goods = new List<Good>();
            Shops = new List<ShopCategory>();
        }

        #endregion
    }
}
