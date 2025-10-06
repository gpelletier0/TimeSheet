using TimeSheet.ViewModels;
using ClientViewModel = TimeSheet.ViewModels.Clients.ClientViewModel;

namespace TimeSheet.Views.Clients;

public partial class ClientPage : ValidatorContentPage<ClientViewModel> {
    public ClientPage(ClientViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}