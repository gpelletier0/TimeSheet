using System.Reflection;
using Mapster;
using TimeSheet.Extensions;
using TimeSheet.Interfaces;

namespace TimeSheet.Configs;

public class MappingConfig : IConfigStrategy {

    public IServiceCollection Configure(IServiceCollection services) {
        services.AddMapster();

        return services;
    }
}