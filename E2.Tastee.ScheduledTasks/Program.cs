using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using E2.Tastee.Common;
using E2.Tastee.Contracts.Services.Interfaces;
using E2.Tastee.DependencyConfigurations;
using E2.Tastee.Automapper;
using System;
using Microsoft.Extensions.Logging;
using System.IO;
using Serilog.Events;
using System.Threading.Tasks;

namespace SHB.ScheduledTasks
{
    class Program
    {
        private static IConfigurationRoot _configuration;
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            try
            {
                Bootstrap();
                var success = RunTasks(args).Result;
                if (success)
                    Log.Information("Successful completion");
                else
                    Log.Warning("UNSuccessful completion");
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Log.Error(e, "Unexpected failure");
                Environment.Exit(1);
            }
        }

        private async static Task<bool> RunTasks(string[] args)
        {
            if (args.Length != 1)
                throw new ArgumentException("Usage: SHB.ScheduledTasks.exe [taskName]");

            Log.Debug("Attempting to execute " + args[0]);
            switch (args[0].ToLower())
            {
                // schedule Friday morning at 8AM
                case "test":
                    // var reportService = _serviceProvider.GetService<IReportService>();
                    var userService = _serviceProvider.GetService<IUserService>();
                    var user = await userService.GetUserAsync(2, null);
                    Log.Debug($"Loaded {user.FullName}");
                    return true;

                default:
                    throw new ArgumentException("Unknown argument", args[0]);
            }
            throw new ApplicationException(string.Format("SHB.ScheduledTasks.RunTask: failed to execute task {0} - missing implementation?", args[0]));
        }

        private static void Bootstrap()
        {
            var executionDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            initConfiguration(executionDirectory, environmentName);
            initLogging(executionDirectory, environmentName);
            var services = new ServiceCollection();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ModelDtoMapperProfile());
                // mc.AddProfile(new ViewModelMapperProfile());
            });
            services.AddSingleton(mappingConfig.CreateMapper());
            services.AddSingleton(x =>
                AppSettingsFactory.CreateAppSettings<AppSettings>(_configuration));
            ServiceCollectionRegistry.RegisterScopedRepositories(services);
            ServiceCollectionRegistry.RegisterScopedServices(services);
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddSerilog(dispose: true);
            });
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void initConfiguration(string executionDirectory, string environmentName)
        {
            IConfigurationBuilder builder;
            try
            {
                if (!String.IsNullOrEmpty(environmentName)
                    && File.Exists(Path.Combine(executionDirectory, $"appsettings.{environmentName}.json")))
                {
                    builder = new ConfigurationBuilder()
                        .SetBasePath(executionDirectory)
                        .AddJsonFile($"appsettings.{environmentName}.json");
                }
                else if (File.Exists(Path.Combine(executionDirectory, "appsettings.json")))
                {
                    if (!String.IsNullOrEmpty(environmentName))
                    {
                        string warningMessage = $"Could not find appsettings.{environmentName}.json at location: [{executionDirectory}]";
                        Log.Warning(warningMessage);
                        Console.WriteLine(warningMessage);
                    }
                    Log.Information("Using configuration file appsettings.json in "  + executionDirectory);
                    builder = new ConfigurationBuilder()
                        .SetBasePath(executionDirectory)
                        .AddJsonFile("appsettings.json");
                } else
                {
                    throw new ApplicationException("No configuration file could be found");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not load the appropriate appsettings.json file");
                Console.WriteLine("Attempted file location: " + Directory.GetCurrentDirectory());
                throw;
            }
            _configuration = builder.Build();
        }

        private static void initLogging(string executionDirectory, string environmentName)
        {
            string path = Path.Combine(executionDirectory, "Log", "shb-scheduled-tasks.log");
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Debug)
                .WriteTo.Console()
                .WriteTo.File(path,
                    fileSizeLimitBytes: 1_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();
        }
    }
}
