using System;
using System.Collections.Generic;
using System.Text;

namespace E2.Tastee.Common.Dtos
{
    public class CacheConfigDto
    {
        public string Key { get; set; }
        public int ExpiryMinutes { get; set; }
    }
}
