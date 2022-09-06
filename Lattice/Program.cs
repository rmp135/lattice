using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Lattice;

await new HostBuilder()
    .ConfigureServices(services =>
    {
        services.RegisterAll(new[] { Assembly.GetAssembly(typeof(Program))! });
    })
    .Build()
    .Services.GetService<Main>()!
    .RunAsync();
