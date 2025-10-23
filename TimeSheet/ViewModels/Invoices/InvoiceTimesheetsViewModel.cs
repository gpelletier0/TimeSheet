using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeSheet.Interfaces;
using TimeSheet.Models;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;

namespace TimeSheet.ViewModels.Invoices;

public partial class InvoiceTimesheetsViewModel : ObservableValidatorViewModel {

    [ObservableProperty]
    private TimePeriod _selectedPeriodFilter = TimePeriod.Month;

    [ObservableProperty]
    private Month _selectedMonthFilter = Month.Current;

    [ObservableProperty]
    private ObservableCollection<InvoiceTimesheetDto> _invoiceTimesheetDtos = [];

    private readonly IRepository<Invoice> _invoiceRepo;
    private readonly IRepository<Timesheet> _timesheetRepo;

    public TimePeriod[] TimePeriodFilter => Enum.GetValues<TimePeriod>();
    public Month[] MonthFilter => Enum.GetValues<Month>();

    public List<string> YearFilter => _timesheetRepo.GetAll<string>(new DistinctTimeSpec() {
        ColumnName = nameof(Timesheet.Date),
        TableName = _timesheetRepo.GetTableName() ?? "Timesheets",
        FormatSpecifier = "Y"
    });

    public InvoiceTimesheetsViewModel(IRepository<Invoice> invoiceRepo, IRepository<Timesheet> timesheetRepo) {
        _invoiceRepo = invoiceRepo;
        _timesheetRepo = timesheetRepo;

        CanDelete = false;
    }

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(InvoiceTimesheetDto.Id), out var obj) && obj is int id) {
            Id = id;
        }
    }

    protected override async Task OnAppearingAsync() {
        var invoiceDto = await _invoiceRepo.FindAsync<InvoiceDto>(Id);
        if (invoiceDto is null) {
            return;
        }

        Title = $"{invoiceDto.Number} Timesheets";

        var projectIds = JsonSerializer.Deserialize<HashSet<int>>(invoiceDto.ProjectIdArray) ?? [];
        await SetInvoiceTimesheetDtos(projectIds);

        var timesheetIds = JsonSerializer.Deserialize<HashSet<int>>(invoiceDto.TimesheetIdArray) ?? [];
        SetSelectedTimesheets(timesheetIds);
    }

    [RelayCommand]
    private void OnSelectAllButton() {
        foreach (var dto in InvoiceTimesheetDtos) {
            dto.IsChecked = true;
        }
    }

    private async Task SetInvoiceTimesheetDtos(HashSet<int> projectIs) {
        var invoiceTimesheetDtos = await _timesheetRepo.ListAsync<InvoiceTimesheetDto>(
            new InvoiceTimesheetsSpec {
                TimeFilter = SelectedPeriodFilter,
                StartDate = DateTime.UtcNow,
                ProjectIds = projectIs
            });

        InvoiceTimesheetDtos = new ObservableCollection<InvoiceTimesheetDto>(invoiceTimesheetDtos);
    }

    private void SetSelectedTimesheets(HashSet<int> timesheetIds) {
        foreach (var dto in InvoiceTimesheetDtos) {
            dto.IsChecked = timesheetIds.Contains(dto.Id);
        }
    }
}