using System.Reflection;
using TimeSheet.Interfaces;

namespace TimeSheet.Extensions;

internal static class ServiceCollectionExtensions {
    public static IServiceCollection AddConfigStrategies(this IServiceCollection services, Assembly assembly) {
        var strategyTypes = assembly.GetTypes()
            .Where(t => typeof(IConfigStrategy).IsAssignableFrom(t) 
                        && t is { IsClass: true, IsAbstract: false });

        foreach (var type in strategyTypes) {
            services.AddTransient(typeof(IConfigStrategy), type);
        }

        using var provider = services.BuildServiceProvider();
        var strategies = provider.GetServices<IConfigStrategy>();

        foreach (var strategy in strategies) {
            strategy.Configure(services);
        }

        return services;
    }
}