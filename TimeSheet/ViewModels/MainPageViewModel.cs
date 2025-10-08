using CommunityToolkit.Mvvm.Input;
using TimeSheet.ViewModels.Invoices;
using TimeSheet.Views.Clients;
using TimeSheet.Views.Projects;
using TimeSheet.Views.Timesheets;

namespace TimeSheet.ViewModels {
    public partial class MainPageViewModel : ObservableViewModel {

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