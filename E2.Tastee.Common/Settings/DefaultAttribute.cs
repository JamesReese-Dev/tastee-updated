using System;

namespace E2.Tastee.Common
{
    /// <summary>
    /// Fallback default string value for application settings if environment variable or json settings are missing
    /// </summary>
    public class DefaultAttribute : Attribute
    {
        public DefaultAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}