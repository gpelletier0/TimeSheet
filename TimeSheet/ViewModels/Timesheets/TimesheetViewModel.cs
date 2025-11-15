using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapster;
using TimeSheet.Attributes;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;

namespace TimeSheet.ViewModels.Timesheets;

public partial class TimesheetViewModel(
    IRepository<Timesheet> timesheetRepo,
    IRepository<Project> projectRepo,
    IRepository<Status> statusRepo) : ObservableValidatorViewModel {

    [ObservableProperty]
    [Required(ErrorMessage = "Date is required")]
    private DateTime _date = DateTime.Today;

    [ObservableProperty]
    [Required(ErrorMessage = "Start time is required")]
    private TimeSpan _startTime = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    [Required(ErrorMessage = "End time is required")]
    [TimeAfterOrEqual(nameof(StartTime))]
    private TimeSpan _endTime = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    [MaxLength(500, ErrorMessage = "Note cannot be longer than 500 characters")]
    private string? _note;

    [ObservableProperty]
    [Required(ErrorMessage = "Project is required")]
    private IdNameDto? _selectedProjectDto;

    [ObservableProperty]
    [Required(ErrorMessage = "Status is required")]
    private IdNameDto? _selectedStatusDto;

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _projectDtos = [];

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _statusDtos = [];

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(TimesheetsDto.Id), out var obj)
            && obj is int id) {
            Id = id;
        }
    }

    protected override async Task OnAppearingAsync() {
        Title = IsNew ? "New Timesheet" : "Edit Timesheet";
        CanDelete = !IsNew;

        await LoadPickersAsync();
        await GetTimesheetAsync();

        SelectedStatusDto ??= StatusDtos.FirstOrDefault();
    }

    protected override async Task SaveAsync() {
        if (!await ValidateViewModelAsync()) {
            return;
        }

        var dto = this.Adapt<TimesheetDto>();
        dto.ProjectId = SelectedProjectDto?.Id;
        dto.StatusId = SelectedStatusDto?.Id;

        if (IsNew) {
            await timesheetRepo.AddAsync(dto);
        }
        else {
            await timesheetRepo.UpdateAsync(dto);
        }

        await Shell.Current.GoToAsync("..");
    }

    protected override async Task DeleteAsync() {
        if (!await Shell.Current.DisplayAlert("Delete Timesheet", "Are you sure you want to delete this timesheet?", "Yes", "No")) {
            return;
        }

        await timesheetRepo.DeleteAsync(Id);
        await Shell.Current.GoToAsync("..");
    }

    private async Task LoadPickersAsync() {
        var projects = await projectRepo.ListAsync<IdNameDto>();
        ProjectDtos = new ObservableCollection<IdNameDto>(projects);

        var statuses = await statusRepo.ListAsync<IdNameDto>();
        StatusDtos = new ObservableCollection<IdNameDto>(statuses);
    }

    private async Task GetTimesheetAsync() {
        if (IsNew) {
            return;
        }

        var dto = await timesheetRepo.FindAsync<TimesheetDto>(Id);
        if (dto is null) {
            return;
        }

        Date = dto.Date;
        StartTime = dto.StartTime;
        EndTime = dto.EndTime;
        Note = dto.Note;
        SelectedProjectDto = ProjectDtos.SingleOrDefault(p => p.Id == dto.ProjectId);
        SelectedStatusDto = StatusDtos.SingleOrDefault(t => t.Id == dto.StatusId);
    }
}