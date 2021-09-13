using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Event;
using E2.Tastee.Common;
using E2.Tastee.Persistence.NHibernate.Listeners;
using E2.Tastee.Persistence.NHibernate.Mappings;

namespace E2.Tastee.DependencyConfigurations
{
    public class SessionFactoryBuilder
    {
        public ISessionFactory GetSessionFactory(AppSettings appSettings)
        {
            return Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                    .ShowSql()
                    .FormatSql()
                    .ConnectionString(p => p.Is(appSettings.ConnectionString)
                    )
                    .AdoNetBatchSize(20)
                    .DefaultSchema("dbo"))
                .ExposeConfiguration(c =>
                    {
                        // updates seldom performed, fields do not exist in this app
                        // c.SetListener(ListenerType.PreUpdate, new AuditEventListener());
                        c.SetListener(ListenerType.PreInsert, new AuditEventListener());
                        c.SetProperty(NHibernate.Cfg.Environment.CommandTimeout, appSettings.DefaultDatabaseCommandTimeoutSeconds.ToString());
                    }
                )
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AuditEventListener>())
                .BuildSessionFactory();
        }
    }
}