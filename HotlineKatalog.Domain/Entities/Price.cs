using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotlineKatalog.Domain.Entities
{
    public class Price : IEntity<int>
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Good))]
        public int GoodId { get; set; }

        [Required]
        [ForeignKey(nameof(Shop))]
        public int ShopId { get; set; }

        public int Value { get; set; }

        [DataType("DateTime")]
        public DateTime Date { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("Prices")]
        public virtual Shop Shop { get; set; }

        [InverseProperty("Prices")]
        public virtual Good Good { get; set; }

        #endregion
    }
}
