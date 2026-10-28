using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CapitalBraineAndSpine.Data.Abstracts
{
    public abstract class BaseEntityWithoutId
    {
        [DefaultValue("GETUTCDATE()")]
        public virtual DateTime CreatedDate { get; set; }

        [DefaultValue("GETUTCDATE()")]
        public virtual DateTime LastUpdated { get; set; }
    }
}
