using TimeSheet.ViewModels.Timesheets;

namespace TimeSheet.Views.Timesheets;

public partial class TimesheetPage : ValidatorContentPage<TimesheetViewModel> {
    public TimesheetPage(TimesheetViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}