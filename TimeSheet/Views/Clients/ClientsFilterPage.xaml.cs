using TimeSheet.ViewModels.Clients;

namespace TimeSheet.Views.Clients;

public partial class ClientsFilterPage : ValidatorContentPage<ClientsFilterViewModel> {
    public ClientsFilterPage(ClientsFilterViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}