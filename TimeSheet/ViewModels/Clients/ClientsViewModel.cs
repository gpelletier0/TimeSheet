using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeSheet.Interfaces;
using TimeSheet.Models.Dtos;
using TimeSheet.Models.Entities;
using TimeSheet.Specifications;
using TimeSheet.Views.Clients;

namespace TimeSheet.ViewModels.Clients {
    public partial class ClientsViewModel(IRepository<Client> clientRepo) : ObservableViewModel {

        [ObservableProperty]
        private ObservableCollection<ClientsDto> _clientDtos = [];

        [ObservableProperty]
        private ClientsDto _selectedClientsDto;

        [ObservableProperty]
        private string _filterNames = string.Empty;

        private ClientsSpec _clientsSpec = new();

        public override void ApplyQueryAttributes(IDictionary<string, object> query) {
            if (query.TryGetValue(nameof(ClientsSpec), out var obj)
                && obj is ClientsSpec spec) {
                _clientsSpec = spec;
            }
        }

        protected override async Task OnAppearingAsync() {
            FilterNames = _clientsSpec.GetFilterNames();
            await LoadClientsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadClientsAsync() {
            var clients = await clientRepo.ListAsync<ClientsDto>(_clientsSpec);
            ClientDtos = new ObservableCollection<ClientsDto>(clients);
        }

        [RelayCommand]
        private async Task FilterAsync() {
            var parameters = new ShellNavigationQueryParameters { { nameof(ClientsSpec), _clientsSpec } };
            await Shell.Current.GoToAsync(nameof(ClientsFilterPage), parameters);
        }

        [RelayCommand]
        private async Task AddAsync() {
            var navigationParameter = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), 0 } };
            await Shell.Current.GoToAsync(nameof(ClientPage), navigationParameter);
        }

        [RelayCommand]
        private async Task ItemTappedAsync() {
            var navigationParameter = new ShellNavigationQueryParameters { { nameof(BaseDto.Id), SelectedClientsDto.Id } };
            await Shell.Current.GoToAsync(nameof(ClientPage), navigationParameter);
        }
    }
}