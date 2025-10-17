using TimeSheet.ViewModels.Invoices;

namespace TimeSheet.Views.Invoices;

public partial class InvoicePage : ValidatorContentPage<InvoiceViewModel> {
    public InvoicePage(InvoiceViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}