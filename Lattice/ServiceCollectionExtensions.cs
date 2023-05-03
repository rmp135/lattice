using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Lattice;

public interface ILatticeConfiguration
{
    void RegisterPlugins(params ILatticePlugin[] plugins);
}

public class LatticeConfiguration : ILatticeConfiguration
{
    public IEnumerable<ILatticePlugin> Plugins = Enumerable.Empty<ILatticePlugin>();
    
    public void RegisterPlugins(
        params ILatticePlugin[] plugins
    )
    {
        Plugins = plugins;
    }
}

public static class ServiceCollectionExtension
{
    /// <summary>
    /// Injects dependencies required for Lattice.
    /// </summary>
    public static IServiceCollection AddLattice(this IServiceCollection serviceCollection, Action<ILatticeConfiguration>? configure = null)
    {
        var configuration = new LatticeConfiguration();
        configure?.Invoke(configuration);
        var assembly = Assembly.GetAssembly(typeof(ServiceCollectionExtension))!;
        var foundServices = assembly.DefinedTypes.Where(x => x.GetCustomAttribute<ExportAttribute>() != null);
        foreach (var service in foundServices)
        {
            var attribute = service.GetCustomAttribute<ExportAttribute>();
            serviceCollection.Add(new ServiceDescriptor(attribute!.Type, service, ServiceLifetime.Singleton));
        }

        foreach (var plugin in configuration.Plugins)
        {
            serviceCollection.AddSingleton(plugin);
        }

        return serviceCollection;
    }
}