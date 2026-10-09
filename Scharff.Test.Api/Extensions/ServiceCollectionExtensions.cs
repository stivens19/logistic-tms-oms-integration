using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scharff.Test.Application.Configuration;
using System.Text;

namespace Scharff.Test.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Scharff TMS-OMS Integration API",
                    Version = "v1",
                    Description = "Middleware de integración de eventos logísticos TMS a OMS con .NET 10"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Ingrese el token JWT con el prefijo Bearer. Ejemplo: Bearer {token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer"),
                    new List<string>()
                }
            });
            });

            return services;
        }

        public static IServiceCollection AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection(JwtSettings.SectionName);
            services.Configure<JwtSettings>(jwtSection);
            var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

            var signingKey = !string.IsNullOrWhiteSpace(jwtSettings.SecretKey)
                ? Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                : Encoding.UTF8.GetBytes("SuperSecretClavePorDefectoParaDesarrolloLocalTMS2026!");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = !string.IsNullOrWhiteSpace(jwtSettings.Issuer),
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = !string.IsNullOrWhiteSpace(jwtSettings.Audience),
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKey),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();
            return services;
        }
    }
}
