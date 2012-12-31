using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Taikun.SqlServer;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Taikun.Demo.WPF.ViewModels {
  public class DatabasesViewModel : ViewModelBase {
    private readonly IDatabaseManager<SqlServerDatabase> databaseManager;

    public ObservableCollection<SqlServerDatabase> Databases { get; private set; }

    //private SqlServerDatabase selectedDatabase;
    //public SqlServerDatabase selectedDatabase {
    //  get { return selectedDatabase; }
    //  set {
    //    selectedDatabase = value;
    //    RaisePropertyChanged(() => selectedDatabase);
    //  }
    //}

    public RelayCommand<SqlServerDatabase> SelectDatabase { get; private set; } 

    public DatabasesViewModel(IDatabaseManager<SqlServerDatabase> databaseManager) {
      this.databaseManager = databaseManager;
      SelectDatabase = new RelayCommand<SqlServerDatabase>((database) => {
        var databaseSelected = new Events.DatabaseSelected(database);
        Messenger.Default.Send<Events.DatabaseSelected>(databaseSelected);
      });

      if (!IsInDesignMode) {
        Databases = new ObservableCollection<SqlServerDatabase>(databaseManager.GetAllDatabases());
      } else {
        Databases = new ObservableCollection<SqlServerDatabase> {
          new SqlServerDatabase(databaseManager, "Test") {
            Description = "This is a test"
          },
          new SqlServerDatabase(databaseManager, "Test 2") {
            Description = "This is a test"
          },
          new SqlServerDatabase(databaseManager, "Test 3") {
            Description = "This is a test"
          },
          new SqlServerDatabase(databaseManager, "Test 4") {
            Description = "This is a test"
          },
          new SqlServerDatabase(databaseManager, "Test 5") {
            Description = "This is a test"
          }
        };
      }

      Messenger.Default.Register<Events.DatabaseCreated>(this, databaseCreatedEventHandler);
    }

    private void databaseCreatedEventHandler(Events.DatabaseCreated databaseCreatedEvent) {
      Databases.Clear();
      foreach (SqlServerDatabase database in databaseManager.GetAllDatabases()) {
        Databases.Add(database);
      }
    }
  }
}
