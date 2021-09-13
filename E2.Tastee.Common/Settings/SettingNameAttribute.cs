using System;

namespace E2.Tastee.Common
{
    /// <summary>
    /// Optional override attribute for application settings properties
    /// This overrides the following default convention based app setting names:
    /// 1 - property name (for json settings) -> will use override name
    /// 2 - uppercase(property name) for environment variable settings -> will use uppercase(override name)
    /// </summary>
    public class SettingNameAttribute : Attribute
    {
        public SettingNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}