using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TimeSheet.Interfaces;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;
using TimeSheet.Views.Projects;

namespace TimeSheet.ViewModels.Projects;

public partial class ProjectsFilterViewModel(IRepository<Client> clientRepo) : ObservableValidatorViewModel {

    [ObservableProperty]
    private string? _projectName;

    [ObservableProperty]
    private decimal? _hourlyWage;

    [ObservableProperty]
    private ObservableCollection<IdNameDto> _clientIdNameDtos = [];

    [ObservableProperty]
    private IdNameDto _selectedClientIdNameDto;

    private ProjectsSpec _projectsSpec = new();

    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.TryGetValue(nameof(ProjectsSpec), out var obj)
            && obj is ProjectsSpec spec) {
            _projectsSpec = spec;
        }
    }

    protected override async Task OnAppearingAsync() {
        CanDelete = true;

        await LoadPickerAsync();
        GetSpec();
    }

    protected override async Task SaveAsync() {
        SetSpec();
        var parameters = new ShellNavigationQueryParameters { { nameof(ProjectsSpec), _projectsSpec } };
        await Shell.Current.GoToAsync(nameof(ProjectsPage), parameters);
    }

    protected override Task DeleteAsync() {
        ProjectName = null;
        HourlyWage = null;
        SelectedClientIdNameDto = ClientIdNameDtos.First();

        return Task.CompletedTask;
    }

    private async Task LoadPickerAsync() {
        var clients = await clientRepo.ListAsync<IdNameDto>();
        ClientIdNameDtos = new ObservableCollection<IdNameDto>(clients);
        ClientIdNameDtos.Insert(0, new IdNameDto { Id = 0, Name = "All" });
    }

    private void GetSpec() {
        ProjectName = _projectsSpec.Name;
        HourlyWage = _projectsSpec.HourlyWage;
        SelectedClientIdNameDto = ClientIdNameDtos.SingleOrDefault(c => c.Id == _projectsSpec.ClientId) ?? ClientIdNameDtos.First();
    }

    private void SetSpec() {
        _projectsSpec.Name = ProjectName;
        _projectsSpec.HourlyWage = HourlyWage;
        _projectsSpec.ClientId = SelectedClientIdNameDto.Id;
    }
}