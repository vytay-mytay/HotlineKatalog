using System.ComponentModel.DataAnnotations.Schema;

namespace HotlineKatalog.Domain.Entities
{
    public class Specification : IEntity<int>
    {
        #region Properties

        [ForeignKey(nameof(Good))]
        public int Id { get; set; }

        public string Json { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Good Good { get; set; }

        #endregion
    }
}
