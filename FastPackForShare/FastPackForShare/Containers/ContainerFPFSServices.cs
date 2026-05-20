using DocumentFormat.OpenXml.Presentation;
using FastPackForShare.Exceptions;
using FastPackForShare.Extensions;
using FastPackForShare.Interfaces;
using FastPackForShare.Interfaces.Factory;
using FastPackForShare.Models;
using FastPackForShare.Services;
using FastPackForShare.Services.Factory;
using FastPackForShare.SimpleMediator.MicrosofExtensionsDI;
using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;
using System.Threading.RateLimiting;

namespace FastPackForShare.Containers;

public static class ContainerFastPackForShareServices
{
    public static void RegisterDbConnection<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
    {
        /* E o mais indicado a ser utilizado, adotar esse tipo de modelo de conexão nos projetos */
        services.AddDbContextFactory<TContext>(opts => opts.UseSqlServer(connectionString,
        b => b.MinBatchSize(5).MaxBatchSize(50).MigrationsAssembly(typeof(TContext).Assembly.FullName)).
        LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuting })
        .EnableSensitiveDataLogging());

        /* Para serviços basicos, esse tipo de conexão é valido */
        services.AddDbContext<TContext>(opts =>
        opts.UseSqlServer(connectionString,
        b => b.MinBatchSize(5).MaxBatchSize(50).MigrationsAssembly(typeof(TContext).Assembly.FullName)).
        LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuting })
        .EnableSensitiveDataLogging());

        /* E mais performatico que o codigo padrão do DbContext, porém para ser consumido internamente pela API sem serviços externos como mensageria, chamadas background e etc... */
        services.AddDbContextPool<TContext>(opts =>
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
        .AddTransient<IRdStationService, RdStationService>()
        .AddScoped<IMetaService, MetaService>();
    }

    public static void RegisterExternalCors(this IServiceCollection services, string[] corsSettings, string corsPolicyNamePrincipal, string corsPolicyNameOptional = "")
    {
        services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyNamePrincipal, builder =>
            {
                builder
                .WithOrigins(corsSettings)
                .WithMethods("GET", "POST", "PUT", "DELETE")
                .WithHeaders("Content-Type", "Authorization");
            });

            if (GuardClauseExtension.IsNotNullOrWhiteSpace(corsPolicyNameOptional))
            {
                options.AddPolicy(corsPolicyNameOptional, builder =>
                {
                    builder
                    .WithOrigins(corsSettings)
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            }
        });
    }

    public static void RegisterHttpClient(this IServiceCollection services)
    {
        services
        .AddHttpClient("Signed")
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        })
        .AddStandardResilienceHandler(options =>
        {
            // Maximo de 3 tentativas com 2 segundos de intervalo
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.Delay = TimeSpan.FromSeconds(2);

            // Timeout de 2 minuto para cada requisição
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(120);

            // Circuit Breaker: Abre o circuito por 30 segundos após 5 falhas consecutivas
            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.MinimumThroughput = 5;
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

            // Rate Limiting: Limita a 100 requisições por minuto
            options.RateLimiter.DefaultRateLimiterOptions.PermitLimit = 100;
            options.RateLimiter.DefaultRateLimiterOptions.QueueLimit = 50;
            options.RateLimiter.DefaultRateLimiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
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
        services.AddAutoMapper(cfg => { }, assemblies);
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
