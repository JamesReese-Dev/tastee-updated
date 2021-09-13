using System;
using System.Collections.Generic;
using System.Text;

namespace E2.Tastee.Common
{
    public class PaginatedListVM<T>
    {
        public IPagination<T> List { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
    }
}
