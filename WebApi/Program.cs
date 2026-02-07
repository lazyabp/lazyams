using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lazy.Core;
using Lazy.Core.Authorization;
using Lazy.Core.LazyAttribute;
using Lazy.Model.DBContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Reflection;
using System.Text;
using System.Text.Json;
using WebApi.Filters;
using WebApi.Hubs;
using WebApi.Init;
using WebApi.Middlewares;

namespace WebApi;

/// <summary>
/// Provides the entry point for the application and configures the web host, services, middleware, and application
/// startup logic.
/// </summary>
/// <remarks>
/// The Program class is responsible for initializing logging, dependency injection,
/// configuration, authentication, CORS, Swagger, and other core services required by the application. It sets up
/// middleware for exception handling, authentication, and authorization, and configures endpoints for controllers
/// and SignalR hubs. The class also ensures that database seed data is initialized at startup. This class is not
/// intended to be instantiated; all logic is contained within the static Main method.
/// </remarks>
public class Program
{
    public static void Main(string[] args)
    {
        // Early init of NLog to allow startup and exception logging, before host is built
        var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        logger.Debug("init main");
        var defaultPolicy = "AllowAllOrigins";
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseKestrel(options =>
            {
                // Handle requests up to 50 MB
                options.Limits.MaxRequestBodySize = 52428800;
            });
            //autofac
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                containerBuilder.RegisterModule<AutofacModule>();
            });

            builder.Services.AddOptions<JwtSettingConfig>().Bind(builder.Configuration.GetSection(JwtSettingConfig.Section)).ValidateDataAnnotations().ValidateOnStart();

            // Configure response headers to use UTF-8 encoding(non-English)  
            builder.Services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(System.Text.Unicode.UnicodeRanges.All);
            });

            // Add services to the container.
            builder.Services.AddAppCore(builder.Configuration);

            //Add Lazy Application services
            builder.Services.AddApplication();
            builder.Services.AddHttpClient();

            builder.Services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(Program)); });

            builder.Services.AddTransient<ExceptionHandlingMiddleware>();

            builder.Services.AddDbContext<LazyDBContext>(option =>
            {
                var connectString = builder.Configuration["DataBase:ConnectionString"];
                // MySQL
                option.UseMySql(connectString, ServerVersion.AutoDetect(connectString), mysqlOptions =>
                {
                    mysqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
            });

            //Add JWT Authentication
            builder
                .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;

                    options.TokenValidationParameters =
                        new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidIssuer = builder.Configuration["JwtSetting:Issuer"],
                            ValidAudience = builder.Configuration["JwtSetting:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    builder.Configuration["JwtSetting:SecurityKey"]
                                )
                            ),
                        };
                });

            // Add Authorization with custom policy provider
            builder.Services.AddAuthorization();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, LazyAuthorizationPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();

            builder
                .Services.AddControllers(options =>
                {
                    options.Filters.Add<ValidateModelFilter>();
                    options.Filters.Add<UnifiedResultFilter>();
                    //Handle requests up to 50 MB
                    options.Filters.Add(
                        new RequestFormLimitsAttribute() { BufferBodyLengthLimit = 52428800 }
                    );
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    // 遇到循环引用时忽略，不再继续往下抓取
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            // EventBus and MediatR
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            );

            builder.Services.AddSwaggerLazy();

            //CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(defaultPolicy,
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            builder.Services.AddSignalR()
                .AddJsonProtocol(options => {
                    options.PayloadSerializerOptions.WriteIndented = true;
                });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // 全局上下文信息
            GlobalContext.HostingEnvironment = app.Environment;
            GlobalContext.Services = builder.Services;
            GlobalContext.Configuration = app.Configuration;
            GlobalContext.ServiceProvider = app.Services;

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(defaultPolicy);
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            //app.MapHub<FileUploadHub>("/fileUploadHub");
            app.MapHub<ChatHub>("/chat");

            // Configure the HTTP request pipeline.
            //app.UseSwaggerLazy();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerLazy();
            }
            app.UseAuthentication();
            app.UseMiddleware<AuthLoggingMiddleware>();
            app.UseAuthorization();

            app.MapControllers();

            using (var socpe = app.Services.CreateScope())
            {
                var dbSeedDataSevices = socpe.ServiceProvider.GetRequiredService<IEnumerable<IDBSeedDataService>>();

                SortedDictionary<int, IDBSeedDataService> sdSeedData = new SortedDictionary<int, IDBSeedDataService>();

                foreach (var dbSeedDataSevice in dbSeedDataSevices)
                {
                    var orderAttri = dbSeedDataSevice.GetType().GetCustomAttribute<DBSeedDataOrderAttribute>();
                    if (orderAttri != null)
                    {
                        sdSeedData.Add(orderAttri.Order, dbSeedDataSevice);
                    }
                }

                foreach (var item in sdSeedData)
                {
                    item.Value.InitAsync().GetAwaiter().GetResult();
                }
            }

            app.Run();
        }
        catch (Exception exception)
        {
            // NLog: catch setup errors
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }
    }
}
