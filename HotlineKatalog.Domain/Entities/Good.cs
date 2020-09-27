using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotlineKatalog.Domain.Entities
{
    public class Good : IEntity<int>
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("Good")]
        public virtual ICollection<Price> Prices { get; set; }

        [InverseProperty("Good")]
        public virtual Specification Specification { get; set; }

        [InverseProperty("Good")]
        public virtual ICollection<ShopGood> Shops { get; set; }

        [InverseProperty("Goods")]
        public virtual Category Category { get; set; }

        #endregion

        #region Ctors

        public Good()
        {
            Prices = new List<Price>();
            Shops = new List<ShopGood>();
        }

        #endregion
    }
}
