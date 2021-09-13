using System.ComponentModel;

namespace E2.Tastee.Common
{
    public enum TypeOfUserRole
    {
        [Description("Any")]
        Any = -1,
        [Description("UNKNOWN")]
        UNKNOWN = 0,
        Administrator = 1,
        Taster = 2
    }
}	