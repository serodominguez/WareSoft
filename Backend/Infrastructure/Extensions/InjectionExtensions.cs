using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Infrastructure.Persistences.Repositories;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DbContextSystem).Assembly.FullName;
            services.AddDbContext<DbContextSystem>(
                options => options.UseSqlServer(
                    configuration.GetConnectionString("DbConnection"), b => b.MigrationsAssembly(assembly)), ServiceLifetime.Transient);

            services.AddTransient<IBrandsRepository, BrandsRepository>();
            services.AddTransient<ICategoriesRepository, CategoriesRepository>();
            services.AddTransient<IRolesRepository, RolesRepository>();
            services.AddTransient<IStoresRepository, StoresRepository>();
            services.AddTransient<IUsersRepository, UsersRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, JwtTokenService>();

            return services;
        }
    }
}
