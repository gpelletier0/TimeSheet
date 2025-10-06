using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;
using TimeSheet.Views.Projects;

namespace TimeSheet.ViewModels.Projects {
    public partial class ProjectsViewModel(IRepository<Project> projectRepo) : ObservableViewModel {

        [ObservableProperty]
        private ObservableCollection<ProjectsDto> _projectDtos = [];

        [ObservableProperty]
        private ProjectsDto _selectedProjectDto;

        [ObservableProperty]
        private string _filterNames = string.Empty;
        
        private ProjectsSpec _projectsSpec = new();

        public override void ApplyQueryAttributes(IDictionary<string, object> query) {
            if (query.TryGetValue(nameof(ProjectsSpec), out var obj)
                && obj is ProjectsSpec spec) {
                _projectsSpec = spec;
            }
        }

        protected override async Task OnAppearingAsync() {
            FilterNames = _projectsSpec.GetFilterNames();
            await LoadProjectsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadProjectsAsync() {
            var projects = await projectRepo.ListAsync<ProjectsDto>(_projectsSpec);
            ProjectDtos = new ObservableCollection<ProjectsDto>(projects);
        }

        [RelayCommand]
        private async Task FilterAsync() {
            var parameters = new ShellNavigationQueryParameters { { nameof(ProjectsSpec), _projectsSpec } };
            await Shell.Current.GoToAsync(nameof(ProjectsFilterPage), parameters);
        }

        [RelayCommand]
        private async Task AddAsync() {
            var parameters = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), 0 } };
            await Shell.Current.GoToAsync(nameof(ProjectPage), parameters);
        }

        [RelayCommand]
        private async Task ItemTappedAsync() {
            var parameters = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), SelectedProjectDto.Id } };
            await Shell.Current.GoToAsync(nameof(ProjectPage), parameters);
        }
    }
}