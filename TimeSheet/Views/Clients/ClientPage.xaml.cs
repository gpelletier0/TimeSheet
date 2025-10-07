using TimeSheet.ViewModels.Clients;

namespace TimeSheet.Views.Clients;

public partial class ClientPage : ValidatorContentPage<ClientViewModel> {
    public ClientPage(ClientViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}