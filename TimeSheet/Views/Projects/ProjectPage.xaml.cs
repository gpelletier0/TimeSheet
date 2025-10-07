using TimeSheet.ViewModels.Projects;

namespace TimeSheet.Views.Projects;

public partial class ProjectPage : ValidatorContentPage<ProjectViewModel> {
    public ProjectPage(ProjectViewModel viewModel) : base(viewModel){
        InitializeComponent();
    }
}