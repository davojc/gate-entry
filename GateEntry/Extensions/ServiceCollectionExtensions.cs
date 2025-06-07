using GateEntry.Processors;
using GateEntry.Repository;
using GateEntry.Services;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GateEntry.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddSingleton<IPlateDetector, SimpleLPRPlateDetector>();
        services.AddSingleton<IPlateAccessRepository, FilePlateAccessRepository>();

        services.AddUnboundedChannel<CameraImage>(true);
        services.AddUnboundedChannel<DetectedPlate>(true);

        services.AddSingleton<IGateService, GateService>();

        services.AddHostedService<CameraMonitor>();
        services.AddHostedService<PlateDetector>();
        services.AddHostedService<AccessControl>();
        return services;
    }

    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!))
                };
            });

        /*
        services.AddAuthentication("CookieAuth")
            .AddCookie("CookieAuth", options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(1);      // Absolute timeout
                options.SlidingExpiration = true;                       // Renew on activity
                options.Cookie.Name = "AuthCookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use HTTPS
            });
        */

        return services;
    }

    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        // Add policy-based auth setup
        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddStaticFiles(this IServiceCollection services)
    {
        /*
        services.AddSpaStaticFiles(configuration =>
        {
            configuration.RootPath = "wwwroot";
        });
        */

        return services;
    }

    public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        services.AddEndpointsApiExplorer();
        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options => {
            options.AddPolicy(name: "CorsPolicy",
                policy => {
                    policy.WithOrigins("https://localhost:7089")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        return services;
    }
    private static IServiceCollection AddUnboundedChannel<T>(this IServiceCollection services, bool singleWriter = false, bool singleReader = true, bool allowSyncCont = false)
    {
        services.AddSingleton<Channel<T>>(_ => Channel.CreateUnbounded<T>(
            new UnboundedChannelOptions
            {
                SingleWriter = singleWriter,
                SingleReader = singleReader,
                AllowSynchronousContinuations = allowSyncCont
            }));

        return services;
    }
}