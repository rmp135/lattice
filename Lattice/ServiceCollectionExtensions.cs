using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Lattice;

public static class ServiceCollectionExtensions
{
    /// <summary>Registers and injects all services of a particular type</summary>
    /// <typeparam name="T">Servie type to find / inject.</typeparam>
    /// <param name="services">The services collection</param>
    /// <param name="assemblies">The assemblies to find the services in.</param>
    /// <param name="lifetime">The type of service to create.</param>
    /// <remarks>https://medium.com/agilix/asp-net-core-inject-all-instances-of-a-service-interface-64b37b43fdc8</remarks>
    public static void RegisterAllTypes<T>(this IServiceCollection services, IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
        foreach (var type in typesFromAssemblies)
            services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
    }

    /// <summary>Registers all types exported with the <see cref="ExportAttribute"/>.</summary>
    /// <param name="services">The services collections.</param>
    /// <param name="assemblies">The assemblies to find the services in.</param>
    public static void RegisterAll(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var foundServices = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetCustomAttribute<ExportAttribute>() != null));
        foreach (var service in foundServices)
        {
            var attribute = service.GetCustomAttribute<ExportAttribute>();
            var scope = ServiceLifetime.Singleton;
            if (attribute!.Scope == ExportScope.Transient)
            {
                scope = ServiceLifetime.Transient;
            }
            services.Add(new ServiceDescriptor(attribute.Type, service, scope));
        }
    }
}