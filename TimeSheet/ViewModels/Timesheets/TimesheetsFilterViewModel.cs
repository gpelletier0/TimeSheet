using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TimeSheet.Interfaces;
using TimeSheet.Models;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;
using TimeSheet.Views.Timesheets;

namespace TimeSheet.ViewModels.Timesheets;

public partial class TimesheetsFilterViewModel(
    IRepository<Project> projectRepo,
    IRepository<Client> clientRepo,
    IRepository<TimesheetStatus> statusRepo) : ObservableValidatorViewModel {

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _projectIdNameDtos = [];

    [ObservableProperty]
    private IdNameDto _selectedProjectIdNameDto;

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _clientIdNameDtos = [];

    [ObservableProperty]
    private IdNameDto _selectedClientIdNameDto;

    [ObservableProperty]
    private ObservableCollection<CheckName<IdNameDto>> _statusCheckNames = [];

    private TimesheetsSpec _timesheetsSpec = new();

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(TimesheetsSpec), out var obj)
            && obj is TimesheetsSpec spec) {
            _timesheetsSpec = spec;
        }
    }

    protected override async Task OnAppearingAsync() {
        CanDelete = true;

        await LoadPickersAsync();
        await LoadStatusCheckNamesAsync();
        GetSpec();
    }

    protected override async Task SaveAsync() {
        SetSpec();
        var parameters = new ShellNavigationQueryParameters { { nameof(TimesheetsSpec), _timesheetsSpec } };
        await Shell.Current.GoToAsync(nameof(TimesheetsPage), parameters);
    }

    protected override Task DeleteAsync() {
        SelectedProjectIdNameDto = ProjectIdNameDtos.First();
        SelectedClientIdNameDto = ClientIdNameDtos.First();

        foreach (var statusCheckName in StatusCheckNames) {
            statusCheckName.IsChecked = false;
        }

        return Task.CompletedTask;
    }

    private async Task LoadPickersAsync() {
        var projects = await projectRepo.ListAsync<IdNameDto>();
        ProjectIdNameDtos = new ObservableCollection<IdNameDto>(projects);
        ProjectIdNameDtos.Insert(0, new IdNameDto { Id = 0, Name = "All" });

        var clients = await clientRepo.ListAsync<IdNameDto>();
        ClientIdNameDtos = new ObservableCollection<IdNameDto>(clients);
        ClientIdNameDtos.Insert(0, new IdNameDto { Id = 0, Name = "All" });
    }

    private async Task LoadStatusCheckNamesAsync() {
        var statuses = await statusRepo.ListAsync<IdNameDto>();
        var checkNames = statuses.Select(d => new CheckName<IdNameDto> { Value = d });

        StatusCheckNames = new ObservableCollection<CheckName<IdNameDto>>(checkNames);
    }

    private void GetSpec() {
        SelectedProjectIdNameDto = ProjectIdNameDtos.SingleOrDefault(p => p.Id == _timesheetsSpec.ProjectId) ?? ProjectIdNameDtos.First();
        SelectedClientIdNameDto = ClientIdNameDtos.SingleOrDefault(c => c.Id == _timesheetsSpec.ClientId) ?? ClientIdNameDtos.First();

        foreach (var checkName in StatusCheckNames) {
            checkName.IsChecked = _timesheetsSpec.StatusIds.Contains(checkName.Value.Id);
        }
    }

    private void SetSpec() {
        _timesheetsSpec.ProjectId = SelectedProjectIdNameDto.Id;
        _timesheetsSpec.ClientId = SelectedClientIdNameDto.Id;
        _timesheetsSpec.StatusIds = StatusCheckNames
            .Where(c => c.IsChecked)
            .Select(c => c.Value.Id)
            .ToHashSet();
    }
}