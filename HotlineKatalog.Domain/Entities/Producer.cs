using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotlineKatalog.Domain.Entities
{
    public class Producer : IEntity<int>
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("Producer")]
        public virtual ICollection<Good> Goods { get; set; }

        #endregion

        #region Ctors

        public Producer()
        {
            Goods = new List<Good>();
        }

        #endregion

    }
}
