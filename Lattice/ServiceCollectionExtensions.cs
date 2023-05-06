using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Lattice;

public interface ILatticeConfiguration
{
    void RegisterPlugin<T>() where T : ILatticePlugin;
}

public class LatticeConfiguration : ILatticeConfiguration
{
    public readonly IList<Type> Plugins = new List<Type>();
    
    public void RegisterPlugin<T>() where T : ILatticePlugin
    {
        Plugins.Add(typeof(T));
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
            serviceCollection.AddSingleton(typeof(ILatticePlugin), plugin);
        }

        return serviceCollection;
    }
}