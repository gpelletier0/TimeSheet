using TimeSheet.ViewModels;

namespace TimeSheet.Views;

public partial class MainPage : ObservableContentPage<MainPageViewModel> {

    public MainPage(MainPageViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}