using CommunityToolkit.Mvvm.Input;
using TimeSheet.Interfaces;
using TimeSheet.Views.Clients;
using TimeSheet.Views.Invoices;
using TimeSheet.Views.Projects;
using TimeSheet.Views.Timesheets;

namespace TimeSheet.ViewModels {
    public partial class MainPageViewModel(IDatabaseService dbService) : ObservableViewModel {

        protected override async Task LoadAsync() {
            await dbService.InitializeAsync();
        }

        [RelayCommand]
        private async Task GoToClientsAsync() {
            await Shell.Current.GoToAsync(nameof(ClientsPage));
        }

        [RelayCommand]
        private async Task GoToProjectsAsync() {
            await Shell.Current.GoToAsync(nameof(ProjectsPage));
        }

        [RelayCommand]
        private async Task GoToTimesheetsAsync() {
            await Shell.Current.GoToAsync(nameof(TimesheetsPage));
        }

        [RelayCommand]
        private async Task GoToInvoicesAsync() {
            await Shell.Current.GoToAsync(nameof(InvoicesPage));
        }
    }
}