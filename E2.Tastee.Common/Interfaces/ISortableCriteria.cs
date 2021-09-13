using System;
using System.Collections.Generic;
using System.Text;

namespace E2.Tastee.Common
{
    public interface ISortableCriteria
    {
        string SortField { get; set; }
        string SortDirection { get; set; }
    }
}
