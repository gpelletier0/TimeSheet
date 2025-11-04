using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeSheet.Extensions;
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
    IRepository<Project> projectRepo,
    IRepository<Status> statusRepo) : ObservableValidatorViewModel {

    private const string InvoicePrefix = "INV";

    [ReadOnly(true), ObservableProperty] private string _number = string.Empty;
    [ObservableProperty] private ObservableCollection<IdNameDto> _clientDtos = [];

    [ObservableProperty, Required(ErrorMessage = "Client is required")]
    private IdNameDto? _selectedClientDto;

    [ObservableProperty] private ObservableCollection<IdNameDto> _statusDtos = [];

    [ObservableProperty, Required(ErrorMessage = "Status is required")]
    private IdNameDto? _selectedStatusDto;

    [ObservableProperty] private ObservableCollection<CheckName<IdNameDto>> _projectDtos = [];
    [ObservableProperty] private DateTime _issueDate;
    [ObservableProperty] private DateTime _dueDate;
    [ObservableProperty] private string? _comments;

    private HashSet<int> _checkedProjectIds = [];

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
        await LoadStatusesAsync();
        await PopulateInvoiceDataAsync();
    }

    protected override async Task SaveAsync() {
        if (!await ValidateViewModelAsync()) {
            return;
        }

        await UpdateInvoiceAsync();
        await Shell.Current.GoToAsync(nameof(InvoicesPage));
    }

    protected override async Task DeleteAsync() {
        if (!await Shell.Current.DisplayAlert("Delete Invoice", "Are you sure you want to delete this invoice?", "Yes", "No")) {
            return;
        }

        await invoiceRepo.DeleteAsync(Id);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task SelectTimesheetsAsync() {
        await UpdateInvoiceAsync();

        var parameters = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), Id } };
        await Shell.Current.GoToAsync(nameof(InvoiceTimesheetsPage), parameters);
    }

    [RelayCommand]
    private void Print() { }

    private async Task LoadClientsAsync() {
        var clientDtos = await clientRepo.ListAsync<IdNameDto>();
        ClientDtos = new ObservableCollection<IdNameDto>(clientDtos);
    }

    private async Task LoadProjectsAsync(int clientId) {
        var projects = await projectRepo.ListAsync<IdNameDto>(new ProjectsSpec { ClientId = clientId });
        var projectCheckList = projects
            .Select(d => new CheckName<IdNameDto> {
                Value = d,
                IsChecked = _checkedProjectIds.Contains(d.Id)
            })
            .ToList();

        ProjectDtos = new ObservableCollection<CheckName<IdNameDto>>(projectCheckList);
        RegisterPropertyChangeHandlers(ProjectDtos);
    }

    private void RegisterPropertyChangeHandlers(IEnumerable<CheckName<IdNameDto>> checkNames) {
        foreach (var checkName in checkNames) {
            checkName.PropertyChanged += OnProjectCheckChanged;
        }
    }

    private async Task LoadStatusesAsync() {
        var statuses = await statusRepo.ListAsync<IdNameDto>();
        var statusDtos = statuses
            .Where(d => !d.Name.Equals("Opened", StringComparison.OrdinalIgnoreCase));

        StatusDtos = new ObservableCollection<IdNameDto>(statusDtos);
    }

    private void OnProjectCheckChanged(object? sender, PropertyChangedEventArgs e) {
        if (sender is not CheckName<IdNameDto> checkName) {
            return;
        }

        if (checkName.IsChecked) {
            _checkedProjectIds.Add(checkName.Value.Id);
        }
        else {
            _checkedProjectIds.Remove(checkName.Value.Id);
        }
    }

    private async Task PopulateInvoiceDataAsync() {
        var invoiceDto = await invoiceRepo.FindAsync<InvoiceDto>(Id);
        if (invoiceDto is null) {
            Number = GenerateInvoiceNumber();
            IssueDate = DateTime.UtcNow;
            DueDate = DateTime.UtcNow.AddMonths(1);
            SelectedStatusDto = StatusDtos.First();

            return;
        }

        Number = invoiceDto.Number;
        IssueDate = invoiceDto.IssueDate;
        DueDate = invoiceDto.DueDate;
        Comments = invoiceDto.Comments;

        SelectedClientDto = ClientDtos.SingleOrDefault(d => d.Id == invoiceDto.ClientId);
        SelectedStatusDto = StatusDtos.SingleOrDefault(d => d.Id == invoiceDto.StatusId) ?? StatusDtos.First();
        _checkedProjectIds = invoiceDto.ProjectIdArray.JsonDeserialize<HashSet<int>>() ?? [];
    }

    private string GenerateInvoiceNumber() {
        var currentDate = DateTime.UtcNow;
        var invoicePrefix = $"{InvoicePrefix}-{currentDate.Year}-{currentDate.Month:00}";
        var sequenceNumber = GetNextSequenceNumber(invoicePrefix);

        return $"{invoicePrefix}-{sequenceNumber:000}";
    }

    private int GetNextSequenceNumber(string prefix) {
        var spec = new SelectMaxSpec {
            ColumnName = nameof(Invoice.Number),
            Start = -3,
            DataType = SqliteDataType.Integer,
            Increment = 1,
            TableName = invoiceRepo.GetTableName()!,
            Pattern = $"{prefix}%"
        };

        return invoiceRepo.Get<int>(spec);
    }

    private async Task UpdateInvoiceAsync() {
        var invoiceDto = new InvoiceDto {
            Id = Id,
            Number = Number,
            ClientId = SelectedClientDto!.Id,
            ProjectIdArray = JsonSerializer.Serialize(_checkedProjectIds),
            IssueDate = IssueDate,
            DueDate = DueDate,
            Comments = Comments,
            StatusId = SelectedStatusDto!.Id
        };

        await invoiceRepo.UpdateAsync(invoiceDto);
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