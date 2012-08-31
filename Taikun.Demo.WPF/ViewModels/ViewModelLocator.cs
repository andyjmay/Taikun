using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Taikun.Demo.WPF.Properties;
using Taikun.SqlServer;

namespace Taikun.Demo.WPF.ViewModels {
  public class ViewModelLocator {
    public ViewModelLocator() {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);            
      SimpleIoc.Default.Register<IProjectManager>(() => new SqlServerProjectManager(Settings.Default.TaikunDatabase));
      SimpleIoc.Default.Register<MainViewModel>();
      SimpleIoc.Default.Register<ProjectsViewModel>();
      SimpleIoc.Default.Register<CreateProjectViewModel>();
      SimpleIoc.Default.Register<CreateProjectTableViewModel>();
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
  }
}
