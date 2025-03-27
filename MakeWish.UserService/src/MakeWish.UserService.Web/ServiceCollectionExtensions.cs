using System.Text;
using EnsureThat;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Web.Options;
using MakeWish.UserService.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace MakeWish.UserService.Web;

public static class ServiceCollectionExtensions
{
    public static void SetupWeb(this IServiceCollection services, IConfiguration configuration)
    {
        SetupAuthentication(services, configuration);
        SetupAuthorization(services, configuration);
        SetupControllers(services);
        SetupSwagger(services);
    }

    private static void SetupAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IAuthTokenProvider, JwtTokenProvider>();
        
        services.Configure<JwtTokenOptions>(configuration.GetSection(JwtTokenOptions.SectionName));
        
        var jwtTokenOptions = configuration.GetSection(JwtTokenOptions.SectionName).Get<JwtTokenOptions>();
        EnsureArg.IsNotNull(jwtTokenOptions, nameof(jwtTokenOptions));
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                configureOptions =>
                {
                    configureOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenOptions.SecretKey))
                    };
                    configureOptions.RefreshInterval = TimeSpan.FromHours(jwtTokenOptions.RefreshIntervalHours);
                });
    }

    private static void SetupAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();
        
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        services.AddScoped<IUserContext, HttpUserContext>();
        services.Configure<HttpUserContextOptions>(configuration.GetSection(HttpUserContextOptions.SectionName));
    }
    
    private static void SetupControllers(this IServiceCollection services)
    {
        services.AddMvc();
        services.AddControllers();
    }
    
    private static void SetupSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token (without Bearer)",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}