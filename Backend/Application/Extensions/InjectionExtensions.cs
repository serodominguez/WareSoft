using Application.Interfaces;
using Application.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).ToArray();

            foreach (var assembly in assemblies)
            {
                AssemblyScanner
                    .FindValidatorsInAssembly(assembly)
                    .ForEach(result =>
                    {
                        services.AddScoped(result.InterfaceType, result.ValidatorType);
                    });
            }

            services.AddScoped<IBrandsApplication, BrandsApplication>();
            services.AddScoped<ICategoriesApplication, CategoriesApplication>();
            services.AddScoped<IRolesApplication, RolesApplication>();
            services.AddScoped<IStoresApplication, StoresApplication>();
            services.AddScoped<IUsersApplication, UsersApplication>();

            return services;
        }
    }
}
