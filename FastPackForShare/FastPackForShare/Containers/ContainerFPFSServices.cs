using Microsoft.Extensions.DependencyInjection;
using FastPackForShare.Services;
using FastPackForShare.Services.Factory;
using FastPackForShare.Interfaces;
using FastPackForShare.Interfaces.Factory;
using Hangfire;
using Serilog;
using FastPackForShare.SimpleMediator.MicrosofExtensionsDI;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Serilog.Filters;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace FastPackForShare.Containers;

public static class ContainerFastPackForShareServices
{
    public static void RegisterDbConnection<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
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
        .AddScoped<IQRCodeService, QRCodeService>()
        .AddTransient<IMapperService, MapperService>();
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
        services.AddHangfire(x => x.UseSimpleAssemblyNameTypeSerializer()
                                   .UseRecommendedSerializerSettings()
                                   .UseSqlServerStorage(connectionString));
        services.AddHangfireServer();
    }

    public static void RegisterSeriLog(this IServiceCollection services, string serilog)
    {
        Serilog.Log.Logger = new LoggerConfiguration()
        .Enrich.WithProperty("Project", "API")
        .Enrich.WithProperty("Environment", "Local")
        .WriteTo.Seq(serilog).CreateLogger();
        services.AddSingleton(Serilog.Log.Logger);
    }

    public static void RegisterSeriLog(this IServiceCollection services, IConfiguration configuration, string connectionStringLogs)
    {
        Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

        Serilog.Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("CreatedDate", DateTime.Now)
            .Filter.ByIncludingOnly(Matching.WithProperty("Object"))
            .WriteTo.MSSqlServer(connectionString: connectionStringLogs,
            sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
            {
                AutoCreateSqlDatabase = false,
                AutoCreateSqlTable = true,
                TableName = "Logs",
                SchemaName = "dbo",
            },
            columnOptions: GetSqlColumnOptions()

            ).CreateLogger();

        services.AddSingleton(Serilog.Log.Logger);
    }

    public static ColumnOptions GetSqlColumnOptions()
    {
        var colOptions = new ColumnOptions();
        colOptions.Store.Remove(StandardColumn.Properties);
        colOptions.Store.Remove(StandardColumn.MessageTemplate);
        colOptions.Store.Remove(StandardColumn.Message);
        colOptions.Store.Remove(StandardColumn.Exception);
        colOptions.Store.Remove(StandardColumn.TimeStamp);

        colOptions.AdditionalColumns = new Collection<SqlColumn>
        {
            new SqlColumn{ DataType = SqlDbType.VarChar, ColumnName = "Class", DataLength = 100, AllowNull = true},
            new SqlColumn{ DataType = SqlDbType.VarChar, ColumnName = "Method", DataLength = 100, AllowNull = true},
            new SqlColumn{ DataType = SqlDbType.VarChar, ColumnName = "MessageError", DataLength = 2000, AllowNull = true},
            new SqlColumn{ DataType = SqlDbType.VarChar, ColumnName = "Object", AllowNull = true},
            new SqlColumn{ DataType = SqlDbType.DateTime, ColumnName = "CreatedDate", AllowNull = false},
        };

        return colOptions;
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

    public static void RegisterMediator(this IServiceCollection services, string assemblyName)
    {
        var myAssembly = AppDomain.CurrentDomain.Load(assemblyName);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(myAssembly));
    }

    public static void RegisterSimpleMediator(this IServiceCollection services, string assemblyName)
    {
        var myAssembly = AppDomain.CurrentDomain.Load(assemblyName);
        services.AddSimpleMediatR(cfg => cfg.RegisterServicesFromAssemblies(myAssembly));
    }

    public static void RegisterAutoMapper(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddAutoMapper(assemblies);
    }

    public static void RegisterPolicy(this IServiceCollection services)
    {
        services.AddMvcCore(config =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            config.Filters.Add(new AuthorizeFilter(policy));
        }).AddApiExplorer();
    }

    public static IServiceCollection RegisterRedis(this IServiceCollection services, IConfiguration configuration, string hostConfiguration, string instanceName)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = hostConfiguration;
            options.InstanceName = instanceName;
        });

        return services;
    }
}
