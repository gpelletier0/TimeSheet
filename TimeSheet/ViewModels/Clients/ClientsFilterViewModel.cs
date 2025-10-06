using CommunityToolkit.Mvvm.ComponentModel;
using TimeSheet.Specifications;
using TimeSheet.Views.Clients;

namespace TimeSheet.ViewModels.Clients;

public partial class ClientsFilterViewModel : ObservableValidatorViewModel {

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _contactName;

    [ObservableProperty]
    private string? _contactEmail;

    [ObservableProperty]
    private string? _contactPhone;

    private ClientsSpec _clientsSpec = new();

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(ClientsSpec), out var obj)
            && obj is ClientsSpec spec) {
            _clientsSpec = spec;
        }
    }

    protected override Task OnAppearingAsync() {
        CanDelete = true;
        GetSpec();
        
        return Task.CompletedTask;
    }

    protected override async Task SaveAsync() {
        SetSpec();
        var parameters = new ShellNavigationQueryParameters { { nameof(ClientsSpec), _clientsSpec } };
        await Shell.Current.GoToAsync(nameof(ClientsPage), parameters);
    }

    protected override Task DeleteAsync() {
        Name = null;
        ContactName = null;
        ContactEmail = null;
        ContactPhone = null;

        return Task.CompletedTask;
    }

    private void GetSpec() {
        Name = _clientsSpec.Name;
        ContactName = _clientsSpec.ContactName;
        ContactEmail = _clientsSpec.ContactEmail;
        ContactPhone = _clientsSpec.ContactPhone;
    }

    private void SetSpec() {
        _clientsSpec.Name = Name;
        _clientsSpec.ContactName = ContactName;
        _clientsSpec.ContactEmail = ContactEmail;
        _clientsSpec.ContactPhone = ContactPhone;
    }
}