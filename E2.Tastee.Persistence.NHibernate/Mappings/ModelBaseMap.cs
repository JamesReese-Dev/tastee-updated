using FluentNHibernate.Mapping;
using E2.Tastee.Models;

namespace E2.Tastee.Persistence.NHibernate.Mappings
{
    public class ModelBaseMap<T> : ClassMap<T> where T : ModelBase
    {
        public ModelBaseMap(string tableName)
        {
            Id(x => x.Id);
            Table(tableName);
        }
    }
}