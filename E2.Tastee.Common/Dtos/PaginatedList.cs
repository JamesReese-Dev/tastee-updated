using System.Collections.Generic;

namespace E2.Tastee.Common
{
    public class PaginatedList<T>
    {
        public int PageNumber { get; set; }
        public IList<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
