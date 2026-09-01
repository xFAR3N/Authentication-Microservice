using AuthenticationMicroservice.Application.Common.Interfaces;
using AuthenticationMicroservice.Domain.Interfaces;
using AuthenticationMicroservice.Infrastructure.Authentication;
using AuthenticationMicroservice.Infrastructure.Persistence;
using AuthenticationMicroservice.Infrastructure.Persistence.Repositories;
using AuthenticationMicroservice.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthenticationMicroservice.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            services.Configure<JwtOptions>(options => configuration.GetSection("Jwt"));

            return services;
        }
    }
}
