using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotlineKatalog.Domain.Entities
{
    public class Shop : IEntity<int>
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Url { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("Shop")]
        public virtual ICollection<Price> Prices { get; set; }

        [InverseProperty("Shop")]
        public virtual ICollection<ShopGood> Goods { get; set; }

        [InverseProperty("Shop")]
        public virtual ICollection<ShopCategory> Categories { get; set; }

        [InverseProperty("Shop")]
        public virtual ShopTags Tags { get; set; }

        #endregion

        #region Ctors

        public Shop()
        {
            Prices = new List<Price>();
            Categories = new List<ShopCategory>();
            Goods = new List<ShopGood>();
        }

        #endregion
    }
}
