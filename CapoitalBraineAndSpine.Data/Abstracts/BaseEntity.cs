using System.ComponentModel.DataAnnotations;

namespace CapitalBraineAndSpine.Data.Abstracts
{
    /// <summary>
    /// Base class which holds common property needed for all entities.
    /// </summary>
    public abstract class BaseEntity : BaseEntityWithoutId
    {
        [Key]
        public virtual int Id { get; set; }
    }
}
