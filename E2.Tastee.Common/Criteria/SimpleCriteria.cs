using System;
using System.Collections.Generic;
using System.Text;

namespace E2.Tastee.Common
{
    public class SimpleCriteria: PagingCriteria
    {
        public string Name { get; set; }
        public bool? ActiveOnly { get; set; }
        public string SortField { get; set; }
        public string SortDirection { get; set; }
    }
}
