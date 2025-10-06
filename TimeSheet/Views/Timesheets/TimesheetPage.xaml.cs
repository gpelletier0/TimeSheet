using TimeSheet.ViewModels;
using TimesheetViewModel = TimeSheet.ViewModels.Timesheets.TimesheetViewModel;

namespace TimeSheet.Views.Timesheets;

public partial class TimesheetPage : ValidatorContentPage<TimesheetViewModel> {
    public TimesheetPage(TimesheetViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}