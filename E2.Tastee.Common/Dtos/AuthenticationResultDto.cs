using System;

namespace E2.Tastee.Common
{
    public class AuthenticationResultDto
    {
        public string Message { get; set; }
        public bool Success => String.IsNullOrEmpty(Message);
        public UserDto User { get; set; }
    }
}
