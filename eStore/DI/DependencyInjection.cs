
using eStore.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.Implement;
using Services.Interface;
using StackExchange.Redis;
using System.Text;
using DataAccess.Base;

namespace eStore.DI
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddJwtAuthentication(configuration);
            services.AddAuthenticationStateProvider();
            services.AddRedis(configuration);
        }


        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            RedisConfiguration redisSetting = new RedisConfiguration();
            configuration.GetSection("RedisConfiguration").Bind(redisSetting);

            services.AddSingleton(redisSetting);

            if (!redisSetting.Enabled)
                return;

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisSetting.ConnectionString));
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSetting.ConnectionString;
            });
            services.AddSingleton<ICacheService, CacheService>();

        }
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<EStoreContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
        public static void AddAuthenticationStateProvider(this IServiceCollection services)
        {
            services.AddScoped<JwtAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
        }
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            string serectKey = configuration.GetValue<string>("JwtSettings:SecretKey")!;
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetValue<string>("JwtSettings:Issuer"), 
                    ValidAudience = configuration.GetValue<string>("JwtSettings:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serectKey!)) 
                };
            });
        }
    }
}
