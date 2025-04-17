using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FastPackForShare.Services;
using FastPackForShare.Services.Factory;
using FastPackForShare.Interfaces;
using FastPackForShare.Interfaces.Factory;
using Hangfire;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace FastPackForShare.Containers;

public static class ContainerFastPackForShareServices
{
    public static void RegisterDbConnection<TContext>(this IServiceCollection services, TContext xpto, string connectionString) where TContext : DbContext
    {
        services.AddDbContextFactory<TContext>(opts => opts.UseSqlServer(connectionString,
        b => b.MinBatchSize(5).MaxBatchSize(50).MigrationsAssembly(typeof(TContext).Assembly.FullName)).
        LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuting })
        .EnableSensitiveDataLogging());

        services.AddDbContext<TContext>(opts =>
        opts.UseSqlServer(connectionString,
        b => b.MinBatchSize(5).MaxBatchSize(50).MigrationsAssembly(typeof(TContext).Assembly.FullName)).
        LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuting })
        .EnableSensitiveDataLogging());
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services
        .AddScoped(typeof(IMemoryCacheService<>), typeof(MemoryCacheService<>))
        .AddScoped(typeof(IMongoDbService<>), typeof(MongoDbService<>))
        .AddScoped(typeof(IFileReadService<>), typeof(FileReadService<>))
        .AddScoped(typeof(IFileWriteService<>), typeof(FileWriteService<>))
        .AddScoped(typeof(IDataFromApiService<>), typeof(DataFromApiService<>))
        .AddTransient<INotificationMessageService, NotificationMessageService>()
        .AddScoped<ITokenService, TokenService>()
        .AddTransient<IDataProtectionService, DataProtectionService>()
        .AddTransient<IExceptionErrorFactory, ExceptionErrorFactory>()
        .AddScoped<IUserLoggedService, UserLoggedService>()
        .AddTransient<ISeriLogService, SeriLogService>()
        .AddScoped<IRedisService, RedisService>()
        .AddScoped<IQRCodeService, QRCodeService>();
    }

    public static void RegisterCors(this IServiceCollection services, string[] corsSettings)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("APICORS", builder =>
            {
                builder
                .WithOrigins(corsSettings) // Configuração de sites que tem permissão para acessar a API
                .WithMethods("GET", "POST", "PUT", "DELETE") // Configuração de tipos de metodos que serão liberados para consumo GET, POST, PUT, DELETE
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });
    }

    public static void RegisterHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient("Signed")
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });
    }

    public static void RegisterHangFire(this IServiceCollection services, string connectionString)
    {
        services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
        services.AddHangfireServer();
    }

    public static void RegisterSeriLog(this IServiceCollection services, string serilog)
    {
        Serilog.Log.Logger = new LoggerConfiguration()
        .Enrich.WithProperty("Project", "WebNotesAPI")
        .Enrich.WithProperty("Environment", "Local")
        .WriteTo.Seq(serilog).CreateLogger();
        services.AddSingleton(Serilog.Log.Logger);
    }

    public static void RegisterHttpContextAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
    }

    public static void RegisterProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails();
    }

    public static void RegisterMemoryCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
    }

    public static void RegisterCustomMediator(this IServiceCollection services)
    {

    }
}
