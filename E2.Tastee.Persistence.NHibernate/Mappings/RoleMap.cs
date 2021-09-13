using E2.Tastee.Models;

namespace E2.Tastee.Persistence.NHibernate.Mappings
{
    public class RoleMap : SimpleModelMap<Role>
    {
        public RoleMap() : base("Roles")
        {

        }
    }
}