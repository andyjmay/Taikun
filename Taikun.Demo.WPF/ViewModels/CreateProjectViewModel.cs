using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using Taikun.Demo.WPF.Models;
using Taikun.SqlServer;
using System.Data;

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

    public CreateProjectViewModel(IProjectManager projectManager) {
      CreateProject = new RelayCommand(() => {
        IProject project = projectManager.CreateProject(new SqlServerProject {
          DatabaseName = ProjectName,
          Description = ProjectDescription
        });
      }, () => { return (!string.IsNullOrWhiteSpace(ProjectName)); }
      );
    }
  }
}
