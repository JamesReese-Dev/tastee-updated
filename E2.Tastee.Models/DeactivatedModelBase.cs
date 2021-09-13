using E2.Tastee.Common;
using System;

namespace E2.Tastee.Models
{
    public class DeactivatedModelBase : UserManagedBase, ISoftDeletableDto
    {
        public virtual DateTime? DeactivatedAt { get; set; }
        public virtual int? DeactivatedByUserId { get; set; }
    }
}