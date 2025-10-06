using TimeSheet.ViewModels;

namespace TimeSheet.Views;

public abstract class ObservableContentPage<T> : ContentPage where T : ObservableViewModel {
    protected readonly T ViewModel;

    protected ObservableContentPage(T viewModel) {
        BindingContext = viewModel;
        ViewModel = viewModel;
    }
    
    protected override void OnAppearing() {
        base.OnAppearing();
        ViewModel.AppearingCommand.ExecuteAsync(null);
    }
}