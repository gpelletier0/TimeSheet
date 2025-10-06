using TimeSheet.ViewModels;
using ProjectsViewModel = TimeSheet.ViewModels.Projects.ProjectsViewModel;

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