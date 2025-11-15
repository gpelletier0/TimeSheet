using TimeSheet.ViewModels.Invoices;

namespace TimeSheet.Views.Invoices;

public partial class InvoicesFilterPage : ValidatorContentPage<InvoicesFilterViewModel> {
    public InvoicesFilterPage(InvoicesFilterViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}