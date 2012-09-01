using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using Taikun.Demo.WPF.Models;
using Taikun.SqlServer;
using System.Data;
using GalaSoft.MvvmLight.Messaging;

namespace Taikun.Demo.WPF.ViewModels {
  public class CreateProjectViewModel : ViewModelBase {
    private string projectName;
    public string ProjectName {
      get { return projectName; }
      set { 
        projectName = value;
        RaisePropertyChanged(() => ProjectName);
      }
    }

    private string projectDescription;
    public string ProjectDescription {
      get { return projectDescription; }
      set {
        projectDescription = value;
        RaisePropertyChanged(() => ProjectDescription);
      }
    }
        
    public RelayCommand CreateProject { get; private set; }

    private IProjectManager projectManager;

    public CreateProjectViewModel(IProjectManager projectManager) {
      this.projectManager = projectManager;
      CreateProject = new RelayCommand(createProject, () => { return (!string.IsNullOrWhiteSpace(ProjectName)); });
    }

    private void createProject() {
      IProject project = projectManager.CreateProject(new SqlServerProject {
        DatabaseName = ProjectName,
        Description = ProjectDescription
      });
      Messenger.Default.Send<Events.ProjectCreated>(new Events.ProjectCreated(project));
    }
  }
}
