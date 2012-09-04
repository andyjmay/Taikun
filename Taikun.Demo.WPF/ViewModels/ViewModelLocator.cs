using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Taikun.Demo.WPF.Properties;
using Taikun.SqlServer;

namespace Taikun.Demo.WPF.ViewModels {
  public class ViewModelLocator {
    public ViewModelLocator() {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);            
      SimpleIoc.Default.Register<IProjectManager>(() => new SqlServerProjectManager(Settings.Default.TaikunDatabase), true);            
      SimpleIoc.Default.Register<CreateProjectTableViewModel>(true);
      SimpleIoc.Default.Register<ViewProjectTableViewModel>(true);
      SimpleIoc.Default.Register<CreateProjectViewModel>(true);
      SimpleIoc.Default.Register<ProjectTablesViewModel>(true);
      SimpleIoc.Default.Register<ProjectsViewModel>(true);
      SimpleIoc.Default.Register<MainViewModel>(true);
    }

    public MainViewModel Main {
      get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
    }
    
    public ProjectsViewModel Projects {
      get { return ServiceLocator.Current.GetInstance<ProjectsViewModel>(); }
    }

    public CreateProjectViewModel CreateProject {
      get { return ServiceLocator.Current.GetInstance<CreateProjectViewModel>(); }
    }

    public CreateProjectTableViewModel CreateProjectTable {
      get { return ServiceLocator.Current.GetInstance<CreateProjectTableViewModel>(); }
    }

    public ViewProjectTableViewModel ViewProjectTable {
      get { return ServiceLocator.Current.GetInstance<ViewProjectTableViewModel>(); }
    }

    public ProjectTablesViewModel ProjectTables {
      get { return ServiceLocator.Current.GetInstance<ProjectTablesViewModel>(); }
    }
  }
}
