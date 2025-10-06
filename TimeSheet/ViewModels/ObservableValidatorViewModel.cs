using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TimeSheet.ViewModels;

public abstract partial class ObservableValidatorViewModel : ObservableValidator, IQueryAttributable {
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private int _id;

    protected bool IsNew => Id == 0;
    protected bool CanDelete { get; set; }

    public virtual void ApplyQueryAttributes(IDictionary<string, object> query) { }

    [RelayCommand]
    protected virtual async Task OnAppearingAsync() {
        await Task.CompletedTask;
    }

    [RelayCommand]
    protected virtual Task SaveAsync() {
        return Task.CompletedTask;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    protected virtual Task DeleteAsync() {
        return Task.CompletedTask;
    }

    [RelayCommand]
    protected virtual async Task CancelAsync() {
        await Shell.Current.GoToAsync("..");
    }
}