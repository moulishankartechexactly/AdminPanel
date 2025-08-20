using DatabaseLayer.Interfaces;
using DatabaseLayer.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseLayer;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabaseLayerRepositories(this IServiceCollection services)
    {
        // Open generic registration
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Concrete repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
