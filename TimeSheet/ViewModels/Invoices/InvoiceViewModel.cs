using System.Collections.ObjectModel;
using System.ComponentModel;
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
    IRepository<Project> projectRepo) : ObservableValidatorViewModel {

    private const string InvoicePrefix = "INV";

    [ReadOnly(true), ObservableProperty] private string _number = string.Empty;
    [ObservableProperty] private ObservableCollection<IdNameDto> _clientDtos = [];
    [ObservableProperty] private IdNameDto? _selectedClientDto;
    [ObservableProperty] private ObservableCollection<CheckName<IdNameDto>> _projectDtos = [];
    [ObservableProperty] private DateTime _issueDate;
    [ObservableProperty] private DateTime _dueDate;
    [ObservableProperty] private string? _comments;

    private InvoiceDto? _invoiceDto;
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

        _invoiceDto = await invoiceRepo.FindAsync<InvoiceDto>(Id);

        LoadInvoice();
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

    private async void OnProjectCheckChanged(object? sender, PropertyChangedEventArgs e) {
        try {
            if (sender is not CheckName<IdNameDto> checkName) {
                return;
            }

            if (checkName.IsChecked) {
                _checkedProjectIds.Add(checkName.Value.Id);
            }
            else {
                _checkedProjectIds.Remove(checkName.Value.Id);
            }

            await UpdateProjectSelectionAsync();
        }
        catch (Exception ex) {
            Debug.WriteLine(ex);
        }
    }

    private void LoadInvoice() {
        InitializeNewInvoice();
        PopulateInvoiceData();
    }

    private void InitializeNewInvoice() {
        Number = GenerateInvoiceNumber();
        IssueDate = DateTime.UtcNow;
        DueDate = DateTime.UtcNow.AddMonths(1);
    }

    private void PopulateInvoiceData() {
        if (_invoiceDto is null) {
            return;
        }

        _checkedProjectIds = _invoiceDto.ProjectIdArray.JsonDeserialize<HashSet<int>>() ?? [];

        Number = _invoiceDto.Number;
        IssueDate = _invoiceDto.IssueDate;
        DueDate = _invoiceDto.DueDate;
        Comments = _invoiceDto.Comments;

        SelectedClientDto = ClientDtos.SingleOrDefault(c => c.Id == _invoiceDto.ClientId);
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

    private async Task UpdateProjectSelectionAsync() {
        if (_invoiceDto is not null) {
            _invoiceDto.ProjectIdArray = JsonSerializer.Serialize(_checkedProjectIds);
            await invoiceRepo.UpdateAsync(_invoiceDto);
        }
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