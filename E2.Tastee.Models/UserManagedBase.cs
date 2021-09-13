using System;

namespace E2.Tastee.Models
{
    public class UserManagedBase : ModelBase
    {
        public virtual DateTime CreatedAt { get; set; }
        public virtual int CreatedByUserId { get; set; }
    }
}
