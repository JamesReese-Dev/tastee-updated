using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace E2.Tastee.Contracts.Services.Interfaces
{
    public interface ICipherService
    {
        string Encrypt<T>(string value, string password, string salt) where T : SymmetricAlgorithm, new();
        string Decrypt<T>(string text, string password, string salt) where T : SymmetricAlgorithm, new();
        string GenerateSalt(int saltLength = 32);
        string ComputeSHA256Hash(string input, string salt);
        bool SHA256HashMatches(string input, string salt, string hash);
    }
}
