using E2.Tastee.Models;

namespace E2.Tastee.Persistence.NHibernate.Mappings
{
    public class DeactivatedModelBaseMap<T> : UserManagedBaseMap<T> where T : DeactivatedModelBase
    {
        public DeactivatedModelBaseMap(string tableName) : base(tableName)
        {
            Map(x => x.DeactivatedAt).Nullable();
            Map(x => x.DeactivatedByUserId).Nullable();
        }
    }
}