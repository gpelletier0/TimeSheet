using TimeSheet.ViewModels.Projects;

namespace TimeSheet.Views.Projects;

public partial class ProjectsFilterPage : ValidatorContentPage<ProjectsFilterViewModel> {
    public ProjectsFilterPage(ProjectsFilterViewModel viewModel) : base(viewModel) {
        InitializeComponent();
    }
}