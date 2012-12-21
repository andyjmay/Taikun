using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Taikun.Demo.WPF.Properties;
using Taikun.SqlServer;

namespace Taikun.Demo.WPF.ViewModels {
  public class ViewModelLocator {
    public ViewModelLocator() {
      ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);            
      SimpleIoc.Default.Register<IDatabaseManager>(() => new SqlServerDatabaseManager(Settings.Default.TaikunDatabase, createIfNotExists: true), true);
      SimpleIoc.Default.Register<CreateDatabaseTableViewModel>(true);
      SimpleIoc.Default.Register<ViewDatabaseTableViewModel>(true);
      SimpleIoc.Default.Register<CreateDatabaseViewModel>(true);
      SimpleIoc.Default.Register<DatabaseTablesViewModel>(true);
      SimpleIoc.Default.Register<DatabasesViewModel>(true);
      SimpleIoc.Default.Register<MainViewModel>(true);
    }

    public MainViewModel Main {
      get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
    }
    
    public DatabasesViewModel Databases {
      get { return ServiceLocator.Current.GetInstance<DatabasesViewModel>(); }
    }

    public CreateDatabaseViewModel CreateDatabase {
      get { return ServiceLocator.Current.GetInstance<CreateDatabaseViewModel>(); }
    }

    public CreateDatabaseTableViewModel CreateDatabaseTable {
      get { return ServiceLocator.Current.GetInstance<CreateDatabaseTableViewModel>(); }
    }

    public ViewDatabaseTableViewModel ViewDatabaseTable {
      get { return ServiceLocator.Current.GetInstance<ViewDatabaseTableViewModel>(); }
    }

    public DatabaseTablesViewModel DatabaseTables {
      get { return ServiceLocator.Current.GetInstance<DatabaseTablesViewModel>(); }
    }
  }
}
