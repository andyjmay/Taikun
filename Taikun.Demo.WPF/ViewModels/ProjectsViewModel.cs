using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using Taikun.Demo.WPF.Properties;
using Taikun.SqlServer;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Taikun.Demo.WPF.ViewModels {
  public class ProjectsViewModel : ViewModelBase {
    private readonly IProjectManager projectManager;

    public ObservableCollection<IProject> Projects { get; private set; }

    //private SqlServerProject selectedProject;
    //public SqlServerProject SelectedProject {
    //  get { return selectedProject; }
    //  set {
    //    selectedProject = value;
    //    RaisePropertyChanged(() => SelectedProject);
    //  }
    //}

    public RelayCommand<IProject> SelectProject { get; private set; } 

    public ProjectsViewModel(IProjectManager projectManager) {
      this.projectManager = projectManager;
      SelectProject = new RelayCommand<IProject>((project) => {
        var projectSelected = new Events.ProjectSelected(project);
        Messenger.Default.Send<Events.ProjectSelected>(projectSelected);
      });

      if (!IsInDesignMode) {
        Projects = new ObservableCollection<IProject>(projectManager.GetAllProjects());
      } else {
        Projects = new ObservableCollection<IProject> {
          new SqlServerProject {
            DatabaseName = "Test",
            Description = "This is a test"
          },
          new SqlServerProject {
            DatabaseName = "Test 2",
            Description = "This is a test"
          },
          new SqlServerProject {
            DatabaseName = "Test 3",
            Description = "This is a test"
          },
          new SqlServerProject {
            DatabaseName = "Test 4",
            Description = "This is a test"
          },
          new SqlServerProject {
            DatabaseName = "Test 5",
            Description = "This is a test"
          }
        };
      }

      Messenger.Default.Register<Events.ProjectCreated>(this, projectCreatedEventHandler);
    }

    private void projectCreatedEventHandler(Events.ProjectCreated projectCreatedEvent) {
      Projects.Clear();
      foreach (IProject project in projectManager.GetAllProjects()) {
        Projects.Add(project);
      }
    }
  }
}
