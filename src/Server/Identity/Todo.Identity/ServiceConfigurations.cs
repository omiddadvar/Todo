using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Constants;
using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Abstractions.Repositories;
using Todo.Identity.Domain.Entities;
using Todo.Identity.Infrastructure.Data;
using Todo.Identity.Infrastructure.Services;

namespace Todo.Identity;

public static class ServiceConfigurations
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddIdentityServices();
        services.AddJwtAuthentication(configuration);
        services.AddMediatRServices();
        services.AddApplicationServices();
        services.AddOpenApiService();
        services.AddMasstransitService(configuration);
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MainDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<MainDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration[ConfigKeyword.Token.SecretKey]
            ?? throw new InvalidOperationException("JWT Secret Key is not configured");

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
                ValidIssuer = configuration[ConfigKeyword.Token.Issuer],
                ValidAudience = configuration[ConfigKeyword.Token.Audience],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
        return services;
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }

    internal sealed class TodoIdentityDocumentTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            document.Info = new OpenApiInfo
            {
                Title = "Todo Identity API",
                Version = "v1",
                Description = "ASP.NET WebAPI for Todo.Identity service (JWT Bearer Authentication)"
            };
            return Task.CompletedTask;
        }
    }
    internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var scheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Enter your JWT token (without 'Bearer ' prefix)."
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = scheme;

            document.Security ??= new List<OpenApiSecurityRequirement>();
            document.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });

            return Task.CompletedTask;
        }
    }
    private static IServiceCollection AddOpenApiService(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(new TodoIdentityDocumentTransformer());
            options.AddDocumentTransformer(new BearerSecuritySchemeTransformer());
        });
        return services;
    }
    private static IServiceCollection AddMasstransitService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration[ConfigKeyword.RabbitMQ.Host], "/", h =>
                {
                    h.Username(configuration[ConfigKeyword.RabbitMQ.Username]);
                    h.Password(configuration[ConfigKeyword.RabbitMQ.Password]);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
