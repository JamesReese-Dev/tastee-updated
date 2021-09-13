using System;
using System.Collections.Generic;
using System.Text;

namespace E2.Tastee.Contracts.Services.Interfaces
{
    public interface ICacheService
    {
        bool Cache(string key, object o, int timeoutMinutes);
        bool Update(string key, object o, int timeoutMinutes);
        object Retrieve(string key);
        void Evict(string key);
    }
}
