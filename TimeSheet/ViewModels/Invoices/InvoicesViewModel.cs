using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;

namespace TimeSheet.ViewModels.Invoices;

public partial class InvoicesViewModel(IRepository<Invoice> invoiceRepo) : ObservableViewModel {

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _clientIdNameDtos = [];

    [ObservableProperty]
    private IdNameDto _selectedClientIdNameDto;

    [ObservableProperty]
    private ObservableCollection<InvoicesDto> _invoiceDtos = [];

    [ObservableProperty]
    private InvoicesDto _selectedInvoiceDto;

    [ObservableProperty]
    private string _filterNames = string.Empty;

    private InvoicesSpec _invoicesSpec = new();

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(InvoicesSpec), out var obj)
            && obj is InvoicesSpec spec) {
            _invoicesSpec = spec;
        }
    }

    protected override Task OnAppearingAsync() {
        FilterNames = _invoicesSpec.GetFilterNames();
        return base.OnAppearingAsync();
    }

    protected override async Task LoadAsync() {
        var timesheets = await invoiceRepo.ListAsync<InvoicesDto>(_invoicesSpec);
        InvoiceDtos = new ObservableCollection<InvoicesDto>(timesheets);
    }

    [RelayCommand]
    private async Task FilterAsync() {
        var parameters = new ShellNavigationQueryParameters { { nameof(InvoicesSpec), _invoicesSpec } };
        //await Shell.Current.GoToAsync(nameof(InvoicesFilterPage), parameters);
    }

    [RelayCommand]
    private async Task AddAsync() {
        var parameters = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), 0 } };
        //await Shell.Current.GoToAsync(nameof(InvoicePage), parameters);
    }

    [RelayCommand]
    private async Task ItemTappedAsync() {
        var parameters = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), SelectedInvoiceDto.Id } };
        //await Shell.Current.GoToAsync(nameof(InvoicePage), parameters);
    }
}