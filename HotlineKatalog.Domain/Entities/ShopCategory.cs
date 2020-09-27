using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotlineKatalog.Domain.Entities
{
    public class ShopCategory : IEntity<int>
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [Required]
        public int ShopId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [MaxLength(100)]
        public string Url { get; set; }

        #endregion

        #region Navigation properties

        [ForeignKey(nameof(ShopId))]
        [InverseProperty("Categories")]
        public virtual Shop Shop { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [InverseProperty("Shops")]
        public virtual Category Category { get; set; }

        #endregion
    }
}
