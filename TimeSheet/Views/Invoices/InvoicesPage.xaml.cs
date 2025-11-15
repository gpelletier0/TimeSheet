using TimeSheet.ViewModels.Invoices;

namespace TimeSheet.Views.Invoices;

public partial class InvoicesPage : ObservableContentPage<InvoicesViewModel> {
    public InvoicesPage(InvoicesViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed() {
        Shell.Current.GoToAsync(nameof(MainPage), false);
        return true;
    }
}