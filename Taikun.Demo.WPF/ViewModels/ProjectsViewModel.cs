using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using Taikun.Demo.WPF.Properties;
using Taikun.SqlServer;

namespace Taikun.Demo.WPF.ViewModels {
  public class ProjectsViewModel : ViewModelBase {
    private readonly IProjectManager projectManager;

    public ObservableCollection<IProject> Projects { get; private set; }

    public ProjectsViewModel(IProjectManager projectManager) {
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
    }
  }
}
