using System;
using System.Runtime.Caching;
using System.Collections.Generic;
using System.Text;
using E2.Tastee.Contracts.Services.Interfaces;

namespace E2.Tastee.Services
{
    public class AspNetMemoryCache : ICacheService
    {
        public object Retrieve(string key)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            return memoryCache.Get(key);
        }

        public bool Cache(string key, object value, int timeoutMinutes)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            DateTimeOffset absExpiration = DateTimeOffset.Now.AddMinutes(timeoutMinutes);
            return memoryCache.Add(key, value, absExpiration);
        }

        public bool Update(string key, object value, int timeoutMinutes)
        {
            if (Retrieve(key) != null)
            {
                Evict(key);
            }
            return Cache(key, value, timeoutMinutes);
        }

        public void Evict(string key)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key))
            {
                memoryCache.Remove(key);
            }
        }
    }
}
