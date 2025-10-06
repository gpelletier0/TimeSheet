using TimeSheet.Extensions;
using TimeSheet.Interfaces;
using TimeSheet.ViewModels;

namespace TimeSheet.Configs;

public class ViewModelConfig : IConfigStrategy {

    public IServiceCollection Configure(IServiceCollection services) {
        var observableViewModels = MauiProgram.ExecutingAssembly.GetSubClasseTypes<ObservableViewModel>();
        foreach (var viewModel in observableViewModels) {
            services.AddTransient(viewModel);
        }

        var observableValidatorViewModels = MauiProgram.ExecutingAssembly.GetSubClasseTypes<ObservableValidatorViewModel>();
        foreach (var viewModel in observableValidatorViewModels) {
            services.AddTransient(viewModel);
        }

        return services;
    }
}