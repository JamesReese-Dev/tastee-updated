using System;

namespace E2.Tastee.Common
{
    public class SoftDeletableSimpleDto: ISoftDeletableDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public int? DeactivatedByUserId { get; set; }
    }
}
