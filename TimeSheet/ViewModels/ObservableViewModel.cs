using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TimeSheet.ViewModels;

public partial class ObservableViewModel : ObservableObject, IQueryAttributable {
    [RelayCommand]
    protected virtual Task OnAppearingAsync() {
        return Task.CompletedTask;
    }

    public virtual void ApplyQueryAttributes(IDictionary<string, object> query) { }
}