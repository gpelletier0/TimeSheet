using System.Collections.ObjectModel;
using System.Text.Json;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeSheet.Extensions;
using TimeSheet.Interfaces;
using TimeSheet.Models;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;

namespace TimeSheet.ViewModels.Invoices;

public partial class InvoiceTimesheetsViewModel : ObservableValidatorViewModel {

    [ObservableProperty]
    private TimePeriod[] _timePeriodFilter = Enum.GetValues<TimePeriod>();

    [ObservableProperty]
    private TimePeriod _selectedPeriodFilter = TimePeriod.Month;

    [ObservableProperty]
    private Month[] _monthFilter = Enum.GetValues<Month>();

    [ObservableProperty]
    private Month _selectedMonthFilter = (Month)DateTime.UtcNow.Month;

    [ObservableProperty]
    private string[] _yearFilter = [];

    [ObservableProperty]
    private string _selectedYearFilter = string.Empty;

    [ObservableProperty]
    private ObservableCollection<InvoiceTimesheetDto> _invoiceTimesheetDtos = [];

    private readonly IRepository<Invoice> _invoiceRepo;
    private readonly IRepository<Timesheet> _timesheetRepo;
    private readonly InvoiceTimesheetsSpec _spec = new();
    private HashSet<int> _timesheetIds = [];
    private DateTime _currentDate;
    private bool _isInitialized;

    public InvoiceTimesheetsViewModel(
        IRepository<Invoice> invoiceRepo,
        IRepository<Timesheet> timesheetRepo) {
        _invoiceRepo = invoiceRepo;
        _timesheetRepo = timesheetRepo;
        _currentDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        CanDelete = false;

        LoadYearFilters();
    }

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(InvoiceTimesheetDto.Id), out var obj) && obj is int id) {
            Id = id;
        }
    }

    protected override async Task OnAppearingAsync() {
        await InitializeInvoiceDataAsync();
    }

    [RelayCommand]
    private void SelectAll() {
        foreach (var dto in InvoiceTimesheetDtos) {
            dto.IsChecked = true;
        }
    }

    [RelayCommand]
    private void DeselectAll() {
        foreach (var dto in InvoiceTimesheetDtos) {
            dto.IsChecked = false;
        }
    }

    private async Task InitializeInvoiceDataAsync() {
        var invoiceDto = await _invoiceRepo.FindAsync<InvoiceDto>(Id);
        if (invoiceDto is null) {
            Debug.WriteLine($"Invoice with ID {Id} not found");
            return;
        }

        Title = $"{invoiceDto.Number} Timesheets";

        _spec.ProjectIds = DeserializeJsonArray(invoiceDto.ProjectIdArray);
        _timesheetIds = DeserializeJsonArray(invoiceDto.TimesheetIdArray);

        await LoadInvoiceTimesheetDtosAsync();

        _isInitialized = true;
    }

    private static HashSet<int> DeserializeJsonArray(string jsonArray) {
        try {
            return JsonSerializer.Deserialize<HashSet<int>>(jsonArray) ?? [];
        }
        catch (JsonException e) {
            Debug.WriteLine($"Error deserializing JSON: {e}");
            return [];
        }
    }

    private void LoadYearFilters() {
        var tableName = _timesheetRepo.GetTableName() ?? "Timesheets";
        var spec = new DistinctTimeSpec {
            ColumnName = nameof(Timesheet.Date),
            TableName = tableName,
            Format = "%Y"
        };

        YearFilter = _timesheetRepo.GetAll<string>(spec).ToArray();

        var currentYear = DateTime.UtcNow.Year.ToString();
        SelectedYearFilter = YearFilter.Contains(currentYear)
            ? currentYear
            : YearFilter.LastOrDefault() ?? currentYear;
    }

    private async Task LoadInvoiceTimesheetDtosAsync() {
        SetSpecificationDates();

        var invoiceTimesheetDtos = await _timesheetRepo.ListAsync<InvoiceTimesheetDto>(_spec);
        InvoiceTimesheetDtos = new ObservableCollection<InvoiceTimesheetDto>(invoiceTimesheetDtos);

        ApplyTimesheetSelection();
    }

    private void SetSpecificationDates() {
        var dates = CalculateDateRange();
        _spec.StartDate = dates?.startDate;
        _spec.EndDate = dates?.endDate;
    }

    private void ApplyTimesheetSelection() {
        foreach (var dto in InvoiceTimesheetDtos) {
            dto.IsChecked = _timesheetIds.Contains(dto.Id);
        }
    }

    private (DateTime startDate, DateTime endDate)? CalculateDateRange() {
        return SelectedPeriodFilter switch {
            TimePeriod.All => null,
            TimePeriod.Day => (_currentDate, _currentDate),
            TimePeriod.Week => _currentDate.WeekPeriod(),
            TimePeriod.Month => _currentDate.MonthPeriod(),
            TimePeriod.Year => _currentDate.YearPeriod(),
            _ => null
        };
    }

    private static DateTime CalculateCurrentDate(TimePeriod period) {
        return period switch {
            TimePeriod.All => DateTime.UtcNow,
            TimePeriod.Day => DateTime.UtcNow.Date,
            TimePeriod.Week => DateTime.UtcNow.WeekPeriod().startDate,
            TimePeriod.Month => DateTime.UtcNow.MonthPeriod().startDate,
            TimePeriod.Year => DateTime.UtcNow.YearPeriod().startDate,
            _ => DateTime.UtcNow
        };
    }

    async partial void OnSelectedPeriodFilterChanged(TimePeriod value) {
        try {
            if (!_isInitialized)
                return;

            _currentDate = CalculateCurrentDate(value);
            await LoadInvoiceTimesheetDtosAsync();
        }
        catch (Exception e) {
            Debug.WriteLine(e);
        }
    }

    async partial void OnSelectedMonthFilterChanged(Month value) {
        try {
            if (!_isInitialized)
                return;

            var year = _currentDate.Year;
            _currentDate = new DateTime(year, (int)value, 1);
            await LoadInvoiceTimesheetDtosAsync();
        }
        catch (Exception e) {
            Debug.WriteLine(e);
        }
    }

    async partial void OnSelectedYearFilterChanged(string value) {
        try {
            if (!_isInitialized || string.IsNullOrEmpty(value))
                return;

            if (!int.TryParse(value, out var year))
                return;

            var month = _currentDate.Month;
            _currentDate = new DateTime(year, month, 1);
            await LoadInvoiceTimesheetDtosAsync();
        }
        catch (Exception e) {
            Debug.WriteLine(e);
        }
    }
}