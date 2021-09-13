using System;
using System.IO;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using E2.Tastee.Persistence.Migrations.Migrations;

namespace E2.Tastee.MigrationRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = CreateServices();
            using var scope = serviceProvider.CreateScope();
            if (args.Length > 0 && args[0] == "rollback")
            {
                int steps;
                if (args.Length > 1 && int.TryParse(args[1], out steps))
                {
                    RollbackDatabase(scope.ServiceProvider, steps);
                } else
                {
                    RollbackDatabase(scope.ServiceProvider, 1);
                }
            }
            else
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        private static IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer2016()
                    .WithGlobalConnectionString(GetConnectionString())
                    .ScanIn(typeof(InitialSchema).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static string GetConnectionString()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (String.IsNullOrEmpty(environmentName))
                environmentName = "Development";
            IConfigurationBuilder builder;

            try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.{environmentName}.json");
                if (File.Exists(path))
                {
                    builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile($"appsettings.{environmentName}.json");
                }
                else
                {
                    //log.Warn("Could not find " + $"appsettings.{environmentName}.json");
                    Console.WriteLine("Could not find " + $"appsettings.{environmentName}.json");
                    Console.WriteLine("Attempted file location: " + Directory.GetCurrentDirectory());
                    builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
                }
            }
            catch (Exception ex)
            {
                builder = null;
                Console.WriteLine("Attempted file location: " + Directory.GetCurrentDirectory());
            }
            var config = builder.Build();
            return config.GetConnectionString("Connection");
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            serviceProvider.GetService<IMigrationRunner>().MigrateUp(); //.MigrateDown(20210518102607); //
        }

        private static void RollbackDatabase(IServiceProvider serviceProvider, int steps = 1)
        {
            serviceProvider.GetService<IMigrationRunner>().Rollback(steps);
        }
    }
}

