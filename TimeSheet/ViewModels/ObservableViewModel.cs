using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TimeSheet.ViewModels;

public partial class ObservableViewModel : ObservableObject, IQueryAttributable {
    public virtual void ApplyQueryAttributes(IDictionary<string, object> query) { }

    [RelayCommand]
    protected virtual Task OnAppearingAsync() {
        LoadCommand.ExecuteAsync(null);
        
        return Task.CompletedTask;
    }

    [RelayCommand]
    protected virtual Task LoadAsync() {
        return Task.CompletedTask;
    }
}