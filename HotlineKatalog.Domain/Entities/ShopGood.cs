using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotlineKatalog.Domain.Entities
{
    public class ShopGood : IEntity<int>
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [Required]
        public int ShopId { get; set; }

        [Required]
        public int GoodId { get; set; }

        [MaxLength(100)]
        public string Url { get; set; }

        #endregion

        #region Navigation properties

        [ForeignKey("ShopId")]
        [InverseProperty("Goods")]
        public virtual Shop Shop { get; set; }

        [ForeignKey("GoodId")]
        [InverseProperty("Shops")]
        public virtual Good Good { get; set; }

        #endregion
    }
}
