using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeSheet.Interfaces;
using TimeSheet.Models;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;
using TimeSheet.Views.Invoices;

namespace TimeSheet.ViewModels.Invoices;

public partial class InvoiceViewModel(
    IRepository<Invoice> invoiceRepo,
    IRepository<Client> clientRepo,
    IRepository<Project> projectRepo) : ObservableValidatorViewModel {

    private const string InvoicePrefix = "INV";

    [ReadOnly(true)]
    [ObservableProperty]
    private string _number = string.Empty;

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _clientDtos = [];

    [ObservableProperty]
    private IdNameDto? _selectedClientDto;

    [ObservableProperty]
    private ObservableCollection<CheckName<IdNameDto>> _projectDtos = [];

    [ObservableProperty]
    private DateTime _issueDate;

    [ObservableProperty]
    private DateTime _dueDate;

    [ObservableProperty]
    private string? _comments;

    private List<int> _projectIds = [];
    private int[] _timesheetIds = [];

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(BaseDto.Id), out var obj)
            && obj is int id) {
            Id = id;
        }
    }

    protected override async Task OnAppearingAsync() {
        Title = IsNew ? "New Invoice" : "Edit Invoice";
        CanDelete = !IsNew;

        await LoadClientsAsync();
        await LoadInvoiceAsync();
    }

    [RelayCommand]
    private async Task SelectTimesheetsAsync() {
        var parameters = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), Id } };
        await Shell.Current.GoToAsync(nameof(InvoiceTimesheetsPage), parameters);
    }
    
    private async Task LoadClientsAsync() {
        var clientDtos = await clientRepo.ListAsync<IdNameDto>();
        ClientDtos = new ObservableCollection<IdNameDto>(clientDtos);
    }

    private async Task LoadProjectsAsync(int clientId) {
        var projects = await projectRepo.ListAsync<IdNameDto>(new ProjectsSpec { ClientId = clientId });
        var projectCheckList = projects
            .Select(p => new CheckName<IdNameDto> {
                Value = p,
                IsChecked = _projectIds.Contains(p.Id)
            })
            .ToList();

        ProjectDtos = new ObservableCollection<CheckName<IdNameDto>>(projectCheckList);
    }

    private async Task LoadInvoiceAsync() {
        if (IsNew) {
            InitializeNewInvoice();
            return;
        }

        await PopulateInvoiceDataAsync();
    }

    private void InitializeNewInvoice() {
        Number = GenerateInvoiceNumber();
        IssueDate = DateTime.UtcNow;
        DueDate = DateTime.UtcNow.AddMonths(1);
    }

    private async Task PopulateInvoiceDataAsync() {
        var invoiceDto = await invoiceRepo.FindAsync<InvoiceDto>(Id);
        if (invoiceDto is null) {
            return;
        }

        var clientDto = await clientRepo.FindAsync<ClientDto>(invoiceDto.ClientId);
        if (clientDto is null) {
            return;
        }

        _projectIds = JsonSerializer.Deserialize<List<int>>(invoiceDto.ProjectIdArray) ?? [];
        _timesheetIds = JsonSerializer.Deserialize<int[]>(invoiceDto.TimesheetIdArray) ?? [];

        Number = invoiceDto.Number;
        SelectedClientDto = ClientDtos.SingleOrDefault(c => c.Id == clientDto.Id);
        IssueDate = invoiceDto.IssueDate;
        DueDate = invoiceDto.DueDate;
        Comments = invoiceDto.Comments;
    }

    private string GenerateInvoiceNumber() {
        var currentDate = DateTime.UtcNow;
        var invoicePrefix = $"{InvoicePrefix}-{currentDate.Year}-{currentDate.Month}";
        var sequenceNumber = GetNextSequenceNumber(invoicePrefix);

        return $"{invoicePrefix}-{sequenceNumber:000}";
    }

    private int GetNextSequenceNumber(string prefix) {
        var spec = new SelectMaxSpec {
            ColumnName = nameof(Invoice.Number),
            Start = -3,
            DataType = SqlDataType.Integer,
            Increment = 1,
            TableName = invoiceRepo.GetTableName()!,
            Pattern = $"{prefix}%"
        };

        return invoiceRepo.Get<int>(spec);
    }

    async partial void OnSelectedClientDtoChanged(IdNameDto? value) {
        try {
            if (value is null) {
                ProjectDtos.Clear();
                return;
            }

            await LoadProjectsAsync(value.Id);
        }
        catch (Exception e) {
            Debug.WriteLine(e);
        }
    }
}