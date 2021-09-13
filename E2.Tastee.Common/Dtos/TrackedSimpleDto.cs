using System;
using System.Collections.Generic;
using System.Text;

namespace E2.Tastee.Common
{
    public class TrackedSimpleDto : SoftDeletableSimpleDto, ITrackedSimpleDto
    {
        public int CreatedByUserId { get; set; }
    }
}
