using TimeSheet.Data;
using TimeSheet.Interfaces;
using TimeSheet.Services;

namespace TimeSheet.Configs;

public class DatabaseConfig : IConfigStrategy {
    public IServiceCollection Configure(IServiceCollection services) {
        services.AddSingleton<IDatabaseService, DatabaseService>(s => {
            var databaseService = new DatabaseService($"{nameof(TimeSheet)}.db3");
            return databaseService;
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        return services;
    }
}