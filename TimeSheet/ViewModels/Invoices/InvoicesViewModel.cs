using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;
using TimeSheet.Views.Invoices;

namespace TimeSheet.ViewModels.Invoices;

public partial class InvoicesViewModel(
    IRepository<Invoice> invoiceRepo,
    IRepository<Client> clientRepo,
    IRepository<Status> statusRepo) : ObservableViewModel {

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _filters = [];

    [ObservableProperty]
    private IdNameDto _selectedFilter;

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

    private InvoicesSpec _invoicesSpec = new() {
        StatusIds = [statusRepo.FirstIdOrDefault("Invoiced")]
    };

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(InvoicesSpec), out var obj)
            && obj is InvoicesSpec spec) {
            _invoicesSpec = spec;
        }
    }

    protected override async Task LoadAsync() {
        FilterNames = _invoicesSpec.GetFilterNames();
        await LoadPickerAsync();
    }

    [RelayCommand]
    private async Task FilterAsync() {
        var parameters = new ShellNavigationQueryParameters { { nameof(InvoicesSpec), _invoicesSpec } };
        await Shell.Current.GoToAsync(nameof(InvoicesFilterPage), parameters);
    }

    [RelayCommand]
    private async Task AddAsync() {
        var parameters = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), 0 } };
        await Shell.Current.GoToAsync(nameof(InvoicePage), parameters);
    }

    [RelayCommand]
    private async Task ItemTappedAsync() {
        var parameters = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), SelectedInvoiceDto.Id } };
        await Shell.Current.GoToAsync(nameof(InvoicePage), parameters);
    }

    private async Task LoadPickerAsync() {
        var clientIdNameDtos = await clientRepo.ListAsync<IdNameDto>();
        clientIdNameDtos.Insert(0, new IdNameDto { Id = 0, Name = "All" });
        
        Filters = new ObservableCollection<IdNameDto>(clientIdNameDtos);
        SelectedFilter = Filters.FirstOrDefault(dto => dto.Id == _invoicesSpec.ClientId) ?? Filters.First();
    }

    private async Task PopulateInvoicesAsync() {
        var invoicesDtos = await invoiceRepo.ListAsync<InvoicesDto>(_invoicesSpec);
        InvoiceDtos = new ObservableCollection<InvoicesDto>(invoicesDtos);
    }

    async partial void OnSelectedFilterChanged(IdNameDto value) {
        try {
            _invoicesSpec.ClientId = value.Id;
            await PopulateInvoicesAsync();
        }
        catch (Exception e) {
            Debug.WriteLine(e);
        }
    }
}