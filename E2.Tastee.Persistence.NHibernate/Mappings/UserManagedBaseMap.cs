using E2.Tastee.Models;

namespace E2.Tastee.Persistence.NHibernate.Mappings
{
    public class UserManagedBaseMap<T> : ModelBaseMap<T> where T : UserManagedBase
    {
        public UserManagedBaseMap(string tableName) : base(tableName)
        {
            Map(x => x.CreatedAt);
            Map(x => x.CreatedByUserId);
        }
    }
}
