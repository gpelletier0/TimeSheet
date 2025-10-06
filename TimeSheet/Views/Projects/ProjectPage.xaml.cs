using TimeSheet.ViewModels;
using ProjectViewModel = TimeSheet.ViewModels.Projects.ProjectViewModel;

namespace TimeSheet.Views.Projects;

public partial class ProjectPage : ValidatorContentPage<ProjectViewModel> {
    public ProjectPage(ProjectViewModel viewModel) : base(viewModel){
        InitializeComponent();
    }
}