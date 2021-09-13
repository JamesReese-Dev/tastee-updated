using E2.Tastee.Common;
using E2.Tastee.Common.Extensions;

namespace E2.Tastee.Models
{
    public class UserRole : DeactivatedModelBase
    {
        public virtual int UserId { get; set; }
        public virtual TypeOfUserRole TypeOfUserRole { get; set; }
        public virtual string RoleName { get { return TypeOfUserRole.Description(); } }
    }
}
