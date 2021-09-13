using E2.Tastee.Common;
using E2.Tastee.Models;

namespace E2.Tastee.Persistence.NHibernate.Mappings
{
    public class UserRoleMap : DeactivatedModelBaseMap<UserRole>
    {
        public UserRoleMap() : base("UserRoles")
        {
            Map(x => x.UserId);
            Map(x => x.TypeOfUserRole).Column("RoleId").CustomType<TypeOfUserRole>();
        }
    }
}
