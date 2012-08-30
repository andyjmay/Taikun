using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Taikun.Demo.WPF.ViewModels {
  public class ViewModelLocator {
    public ViewModelLocator() {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
      SimpleIoc.Default.Register<MainViewModel>();
      SimpleIoc.Default.Register<ProjectsViewModel>();
      SimpleIoc.Default.Register<CreateProjectViewModel>();
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
  }
}
