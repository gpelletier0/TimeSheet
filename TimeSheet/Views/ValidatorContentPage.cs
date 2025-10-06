using TimeSheet.ViewModels;

namespace TimeSheet.Views;

public abstract class ValidatorContentPage<T> : ContentPage where T : ObservableValidatorViewModel {
    protected readonly T ViewModel;

    protected ValidatorContentPage(T viewModel) {
        BindingContext = viewModel;
        ViewModel = viewModel;
    }
    
    protected override void OnAppearing() {
        base.OnAppearing();
        ViewModel.AppearingCommand.ExecuteAsync(null);
    }
}