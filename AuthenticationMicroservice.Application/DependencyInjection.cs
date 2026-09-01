using System;
using System.Collections.Generic;
using System.Text;
using AuthenticationMicroservice.Application.Common.Interfaces;
using AuthenticationMicroservice.Application.DTOs.Requests;
using AuthenticationMicroservice.Application.Services;
using AuthenticationMicroservice.Domain.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationMicroservice.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IAuthService, AuthService>();

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
