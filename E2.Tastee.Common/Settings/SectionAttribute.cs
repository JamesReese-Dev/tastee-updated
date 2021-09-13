using System;

namespace E2.Tastee.Common
{
    /// <summary>
    /// Overrides the section name for json settings (instead of AppSettings default section)
    /// For ENV var settings, uses prefix namespaced value in the format SECTION_SETTING
    /// </summary>
    public class SectionAttribute : Attribute
    {
        public SectionAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}