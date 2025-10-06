using TimeSheet.ViewModels;
using ClientsViewModel = TimeSheet.ViewModels.Clients.ClientsViewModel;

namespace TimeSheet.Views.Clients;

public partial class ClientsPage : ObservableContentPage<ClientsViewModel> {
    public ClientsPage(ClientsViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed() {
        Shell.Current.GoToAsync(nameof(MainPage), false);
        return true;
    }
}