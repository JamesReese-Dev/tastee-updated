using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using E2.Tastee.DependencyConfigurations;
using E2.Tastee.Web.ActionFilters;
using E2.Tastee.Common;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using E2.Tastee.Automapper;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Linq;

namespace E2.Tastee.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.Filters.Add(new UnhandledExceptionFilter()));
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ModelDtoMapperProfile());
                mc.AddProfile(new ViewModelMapperProfile());
            });
            services.AddSingleton(mappingConfig.CreateMapper());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(x => 
                AppSettingsFactory.CreateAppSettings<AppSettings>(x.GetService<IConfigurationRoot>()));
            ServiceCollectionRegistry.RegisterScopedRepositories(services);
            ServiceCollectionRegistry.RegisterScopedServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var appSettings = serviceProvider.GetService<AppSettings>();
            services.AddMvc();
            services.AddScoped<UnitOfWorkActionFilter>();
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy", x =>
                    x.WithOrigins(appSettings.ValidCorsOrigins.ToArray())
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders(AppConstants.VERSION_HEADER_NAME));
            });
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.JwtSecret)),
                    ValidIssuer = appSettings.JwtValidIssuer,
                    ValidateIssuer = true,
                    ValidateAudience = false
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AppConstants.POLICIES.AnyUser, policy => policy.RequireClaim(ClaimTypes.Name));
                options.AddPolicy(AppConstants.POLICIES.TenantOrSystemAdmin, policy => policy.RequireClaim(ClaimTypes.Role, ((int)TypeOfUserRole.Administrator).ToString()));
                options.AddPolicy(AppConstants.POLICIES.AnyAdmin, policy => policy.RequireAssertion(context =>
                {
                    return context.User.HasClaim(ClaimTypes.Role, ((int)TypeOfUserRole.Administrator).ToString())
                    || context.User.HasClaim(ClaimTypes.Role, ((int)TypeOfUserRole.Administrator).ToString());
                }));

                options.AddPolicy(AppConstants.POLICIES.Participant, policy => policy.RequireClaim(AppConstants.CLAIM_TYPE_PARTICIPANT_ID));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceScopeFactory serviceScopeFactory)
        {
            AddVersionToResponseHeaders(app);
            AddCacheDefeatingHeaders(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(
            //           Path.Combine(Directory.GetCurrentDirectory(), @"Content")),
            //    RequestPath = new PathString("/Content")
            //});
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        private void AddCacheDefeatingHeaders(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    if (!context.Response.Headers.ContainsKey(AppConstants.CACHE_CONTROL_HEADER_NAME))
                        context.Response.Headers.Add(AppConstants.CACHE_CONTROL_HEADER_NAME, "no-cache, no-store, must-revalidate");
                    if (!context.Response.Headers.ContainsKey(AppConstants.EXPIRES_HEADER_NAME))
                        context.Response.Headers.Add(AppConstants.EXPIRES_HEADER_NAME, "0"); // "Sat, 01 Jan 2000 00:00:00 GMT");
                    if (!context.Response.Headers.ContainsKey(AppConstants.PRAGMA_HEADER_NAME))
                        context.Response.Headers.Add(AppConstants.PRAGMA_HEADER_NAME, "no-cache");
                    return Task.FromResult(0);
                });
                await next();
            });
        }

        private void AddVersionToResponseHeaders(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(AppConstants.VERSION_HEADER_NAME, AppConstants.VERSION_NUMBER);
                    return Task.FromResult(0);
                });
                await next();
            });
        }
    }
}
