using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Lattice;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// Injects dependencies required for Lattice.
    /// </summary>
    public static IServiceCollection AddLattice(this IServiceCollection serviceCollection)
    {
        var assembly = Assembly.GetAssembly(typeof(ServiceCollectionExtension))!;
        var foundServices = assembly.DefinedTypes.Where(x => x.GetCustomAttribute<ExportAttribute>() != null);
        foreach (var service in foundServices)
        {
            var attribute = service.GetCustomAttribute<ExportAttribute>();
            serviceCollection.Add(new ServiceDescriptor(attribute!.Type, service, ServiceLifetime.Singleton));
        }

        return serviceCollection;
    }
}