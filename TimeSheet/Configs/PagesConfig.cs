using TimeSheet.Extensions;
using TimeSheet.Interfaces;

namespace TimeSheet.Configs;

public class PagesConfig : IConfigStrategy {

    public IServiceCollection Configure(IServiceCollection services) {
        var types = MauiProgram.ExecutingAssembly.GetSubClasseTypes<ContentPage>();
        foreach (var type in types) {
            services.AddTransient(type);
            Routing.RegisterRoute(type.Name, type);
        }
      
        return services;
    }
}