using Mapster;
using TimeSheet.Interfaces;

namespace TimeSheet.Configs;

public class MappingConfig : IConfigStrategy {

    public IServiceCollection Configure(IServiceCollection services) {
        services.AddMapster();

        return services;
    }
}