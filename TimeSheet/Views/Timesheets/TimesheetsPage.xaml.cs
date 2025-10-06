using TimeSheet.ViewModels;
using TimesheetsViewModel = TimeSheet.ViewModels.Timesheets.TimesheetsViewModel;

namespace TimeSheet.Views.Timesheets;

public partial class TimesheetsPage : ObservableContentPage<TimesheetsViewModel> {
    public TimesheetsPage(TimesheetsViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed() {
        Shell.Current.GoToAsync(nameof(MainPage), false);
        return true;
    }
}