using TimeSheet.ViewModels.Projects;

namespace TimeSheet.Views.Projects;

public partial class ProjectsPage : ObservableContentPage<ProjectsViewModel> {
    public ProjectsPage(ProjectsViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed() {
        Shell.Current.GoToAsync(nameof(MainPage), false);
        return true;
    }
}