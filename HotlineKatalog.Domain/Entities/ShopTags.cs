using System.ComponentModel.DataAnnotations.Schema;

namespace HotlineKatalog.Domain.Entities
{
    public class ShopTags
    {
        #region Properties

        [ForeignKey(nameof(Shop))]
        public int Id { get; set; }

        public string NameTag { get; set; }

        public string PriceTag { get; set; }

        public string SpecificationTag { get; set; }

        public string NextPageTag { get; set; }

        public string GoodUrlTag { get; set; }

        public string PageItemTag { get; set; }

        #endregion

        #region Navigation properties

        [InverseProperty("Tags")]
        public virtual Shop Shop { get; set; }

        #endregion
    }
}
