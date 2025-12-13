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
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using FastPackForShare.Models;
using FastPackForShare.Exceptions;
using Serilog.Events;

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
        .AddTransient<IMapperService, MapperService>()
        .AddTransient<IMapsterService, MapsterService>()
        .AddSingleton<ISendGridService, SendGridService>()
        .AddTransient<IRdStationService, RdStationService>();
    }

    public static void RegisterCors(this IServiceCollection services, string[] corsSettings, string corsPolicyName)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, builder =>
            {
                builder
                .WithOrigins(corsSettings) // Configuração de sites que tem permissão para acessar a API
                .WithMethods("GET", "POST", "PUT", "DELETE")
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

    public static void RegistrarSerilog(this WebApplicationBuilder builder, string connectionStringLogs)
    {
        Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

        var sinkOptions = new MSSqlServerSinkOptions
        {
            TableName = "ControleLogs_Erros",
            AutoCreateSqlTable = false,
            AutoCreateSqlDatabase = false,
            SchemaName = "dbo"
        };

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.MSSqlServer(
                connectionString: connectionStringLogs,
                sinkOptions: sinkOptions,
                restrictedToMinimumLevel: LogEventLevel.Information,
                columnOptions: GetSqlColumnOptions()
            )
            .CreateLogger();

        builder.Host.UseSerilog(logger);
        builder.Services.AddSingleton<Serilog.ILogger>(logger);
    }

    public static ColumnOptions GetSqlColumnOptions()
    {
        var colOptions = new ColumnOptions
        {
            Store = new Collection<StandardColumn>(),
            AdditionalColumns = new Collection<SqlColumn>
            {
            new SqlColumn{ DataType = SqlDbType.VarChar, ColumnName = "Class", DataLength = 100, AllowNull = true},
            new SqlColumn{ DataType = SqlDbType.VarChar, ColumnName = "Method", DataLength = 100, AllowNull = true},
            new SqlColumn{ DataType = SqlDbType.VarChar, ColumnName = "MessageError", DataLength = 2000, AllowNull = true},
            new SqlColumn{ DataType = SqlDbType.VarChar, ColumnName = "Object", AllowNull = true},
            new SqlColumn{ DataType = SqlDbType.DateTime, ColumnName = "CreatedDate", AllowNull = false},
            }
        };

        return colOptions;
    }

    public static void RegisterHttpContextAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
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
        services.AddAutoMapper(cfg => {}, assemblies);
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

    public static void RegisterRedis(this IServiceCollection services, IConfiguration configuration, string hostConfiguration, string instanceName)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = hostConfiguration;
            options.InstanceName = instanceName;
        });
    }

    public static void RegisterFluentValidation(this IServiceCollection services, string assemblyName)
    {
        var myAssembly = AppDomain.CurrentDomain.Load(assemblyName);
        services.AddValidatorsFromAssembly(myAssembly);
    }

    public static void RegisterOAuth(this IServiceCollection services, OAuthModel oAuthModel)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "OAuth";
            options.DefaultChallengeScheme = "OAuth";
        })
        .AddOAuth(oAuthModel.OAuthPolicyName, options =>
        {
            options.ClientId = oAuthModel.ClientId;
            options.ClientSecret = oAuthModel.ClientSecret;
            options.CallbackPath = oAuthModel.CallbackPath;
            options.AuthorizationEndpoint = oAuthModel.AuthorizationEndpoint;
            options.TokenEndpoint = oAuthModel.TokenEndpoint;
            options.SaveTokens = oAuthModel.SaveTokens;
        });
    }

    public static void RegisterRateLimit(this IServiceCollection services, string policyName)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(policyName, limiterOptions =>
            {
                limiterOptions.PermitLimit = 5; // Máximo de 5 requisições
                limiterOptions.Window = TimeSpan.FromSeconds(10); // Por janela de 10 segundos
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 2; // Máximo de 2 requisições na fila
            });
        });
    }

    public static void RegisterGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }
}
