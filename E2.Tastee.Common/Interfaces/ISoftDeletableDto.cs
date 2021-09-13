using System;

namespace E2.Tastee.Common
{
    public interface ISoftDeletableDto
    {
        DateTime? DeactivatedAt { get; set; }
        int? DeactivatedByUserId { get; set; }
    }
}
