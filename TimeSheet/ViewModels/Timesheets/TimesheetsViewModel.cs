using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeSheet.Interfaces;
using TimeSheet.Models;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;
using TimeSheet.Views.Timesheets;

namespace TimeSheet.ViewModels.Timesheets {
    public partial class TimesheetsViewModel(IRepository<Timesheet> timesheetRepo) : ObservableViewModel {

        [ObservableProperty]
        private ObservableCollection<string> _timeFilters = new(Enum.GetNames<TimePeriod>());

        [ObservableProperty]
        private TimePeriod _selectedTimeFilter;

        [ObservableProperty]
        private ObservableCollection<TimesheetsDto> _timesheetDtos = [];

        [ObservableProperty]
        private TimesheetsDto _selectedTimesheetsDto;

        [ObservableProperty]
        private string _filterNames = string.Empty;

        private TimesheetsSpec _timesheetsSpec = new();

        public override void ApplyQueryAttributes(IDictionary<string, object> query) {
            if (query.TryGetValue(nameof(TimesheetsSpec), out var obj)
                && obj is TimesheetsSpec spec) {
                _timesheetsSpec = spec;
            }
        }

        protected override Task OnAppearingAsync() {
            SelectedTimeFilter = _timesheetsSpec.TimeFilter;
            FilterNames = _timesheetsSpec.GetFilterNames();
            LoadTimesheetsCommand.ExecuteAsync(null);
            
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task LoadTimesheetsAsync() {
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
        
        partial void OnSelectedTimeFilterChanged(TimePeriod value) {
            _timesheetsSpec.TimeFilter = value;
        }
    }
}