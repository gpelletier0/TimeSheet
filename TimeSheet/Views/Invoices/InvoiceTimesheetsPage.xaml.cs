using TimeSheet.ViewModels.Invoices;

namespace TimeSheet.Views.Invoices;

public partial class InvoiceTimesheetsPage : ValidatorContentPage<InvoiceTimesheetsViewModel> {
    public InvoiceTimesheetsPage(InvoiceTimesheetsViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}