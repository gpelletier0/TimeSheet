using TimeSheet.ViewModels.Timesheets;

namespace TimeSheet.Views.Timesheets;

public partial class TimesheetsFilterPage : ValidatorContentPage<TimesheetsFilterViewModel> {
    public TimesheetsFilterPage(TimesheetsFilterViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}