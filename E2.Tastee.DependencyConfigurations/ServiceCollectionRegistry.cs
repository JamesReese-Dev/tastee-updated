using System;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using E2.Tastee.Common;
using E2.Tastee.Contracts.Persistence;
using E2.Tastee.Contracts.Services.Interfaces;
using E2.Tastee.Persistence.NHibernate.Repositories;
using E2.Tastee.Services;
using IRefService = E2.Tastee.Contracts.Services.Interfaces.IReferenceService;

namespace E2.Tastee.DependencyConfigurations
{
    public class ServiceCollectionRegistry
    {
        public static void RegisterScopedServices(IServiceCollection services)
        {
            services.AddScoped<ICipherService, CipherService>();
            services.AddScoped<IRefService, ReferenceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICacheService, AspNetMemoryCache>();
            services.AddScoped(x =>
            {
                var appSettings = x.GetService<AppSettings>();
                SmtpClient client = new SmtpClient(appSettings.SmtpServer, appSettings.SmtpPort);
                if (appSettings.SmtpRequiresAuthentication)
                {
                    client.Credentials = new System.Net.NetworkCredential(appSettings.SmtpUsername, appSettings.SmtpPassword);
                }
                client.EnableSsl = appSettings.SmtpUseSsl;
                return client;
            });
            services.AddScoped<IAdminService, AdminService>();
        }

        public static void RegisterScopedRepositories(IServiceCollection services)
        {
            services.AddSingleton(x => new SessionFactoryBuilder().GetSessionFactory(x.GetService<AppSettings>()));
            services.AddScoped(factory => factory.GetService<ISessionFactory>().OpenStatelessSession());
            services.AddScoped(x =>
            {
                var sessionFactory = x.GetService<ISessionFactory>();
                if (sessionFactory == null)
                {
                    throw new Exception("Could not initialize a session factory before warming up the ORM");
                }
                return sessionFactory.OpenSession();
            });
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IReferenceRepository, ReferenceRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}