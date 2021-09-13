using E2.Tastee.Models;

namespace E2.Tastee.Persistence.NHibernate.Mappings
{
    public class SimpleModelMap<T> : ModelBaseMap<T> where T : SimpleModel
    {
        public SimpleModelMap(string tableName) : base(tableName)
        {
            Map(x => x.Name);
        }
    }
}
