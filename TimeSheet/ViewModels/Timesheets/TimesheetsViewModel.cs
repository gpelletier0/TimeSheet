using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeSheet.Interfaces;
using TimeSheet.Models;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;
using TimeSheet.Views.Timesheets;

namespace TimeSheet.ViewModels.Timesheets {
    public partial class TimesheetsViewModel(IRepository<Timesheet> timesheetRepo, IRepository<Status> statusRepo) : ObservableViewModel {

        [ObservableProperty]
        private ObservableCollection<string> _filters = new(Enum.GetNames<TimePeriod>());

        [ObservableProperty]
        private TimePeriod _selectedFilter;

        [ObservableProperty]
        private ObservableCollection<TimesheetsDto> _timesheetDtos = [];

        [ObservableProperty]
        private TimesheetsDto _selectedTimesheetsDto;

        [ObservableProperty]
        private string _filterNames = string.Empty;

        private TimesheetsSpec _timesheetsSpec = new() {
            StatusIds = [statusRepo.FirstIdOrDefault("Opened")]
        };

        public override void ApplyQueryAttributes(IDictionary<string, object> query) {
            if (query.TryGetValue(nameof(TimesheetsSpec), out var obj)
                && obj is TimesheetsSpec spec) {
                _timesheetsSpec = spec;
            }
        }

        protected override async Task OnAppearingAsync() {
            SelectedFilter = _timesheetsSpec.TimeFilter;
            FilterNames = _timesheetsSpec.GetFilterNames();
        }

        protected override async Task LoadAsync() {
            var timesheets = await timesheetRepo.ListAsync<TimesheetsDto>(_timesheetsSpec);
            TimesheetDtos = new ObservableCollection<TimesheetsDto>(timesheets);
        }

        [RelayCommand]
        private async Task FilterAsync() {
            var parameters = new ShellNavigationQueryParameters { { nameof(TimesheetsSpec), _timesheetsSpec } };
            await Shell.Current.GoToAsync(nameof(TimesheetsFilterPage), parameters);
        }

        [RelayCommand]
        private async Task AddAsync() {
            var parameters = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), 0 } };
            await Shell.Current.GoToAsync(nameof(TimesheetPage), parameters);
        }

        [RelayCommand]
        private async Task ItemTappedAsync() {
            var parameters = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), SelectedTimesheetsDto.Id } };
            await Shell.Current.GoToAsync(nameof(TimesheetPage), parameters);
        }

        partial void OnSelectedFilterChanged(TimePeriod value) {
            _timesheetsSpec.TimeFilter = value;
            LoadCommand.ExecuteAsync(null);
        }
    }
}