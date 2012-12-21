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
  public class DatabasesViewModel : ViewModelBase {
    private readonly IDatabaseManager databaseManager;

    public ObservableCollection<IDatabase> Databases { get; private set; }

    //private SqlServerDatabase selectedDatabase;
    //public SqlServerDatabase selectedDatabase {
    //  get { return selectedDatabase; }
    //  set {
    //    selectedDatabase = value;
    //    RaisePropertyChanged(() => selectedDatabase);
    //  }
    //}

    public RelayCommand<IDatabase> SelectDatabase { get; private set; } 

    public DatabasesViewModel(IDatabaseManager databaseManager) {
      this.databaseManager = databaseManager;
      SelectDatabase = new RelayCommand<IDatabase>((database) => {
        var databaseSelected = new Events.DatabaseSelected(database);
        Messenger.Default.Send<Events.DatabaseSelected>(databaseSelected);
      });

      if (!IsInDesignMode) {
        Databases = new ObservableCollection<IDatabase>(databaseManager.GetAllDatabases());
      } else {
        Databases = new ObservableCollection<IDatabase> {
          new SqlServerDatabase {
            Name = "Test",
            Description = "This is a test"
          },
          new SqlServerDatabase {
            Name = "Test 2",
            Description = "This is a test"
          },
          new SqlServerDatabase {
            Name = "Test 3",
            Description = "This is a test"
          },
          new SqlServerDatabase {
            Name = "Test 4",
            Description = "This is a test"
          },
          new SqlServerDatabase {
            Name = "Test 5",
            Description = "This is a test"
          }
        };
      }

      Messenger.Default.Register<Events.DatabaseCreated>(this, databaseCreatedEventHandler);
    }

    private void databaseCreatedEventHandler(Events.DatabaseCreated databaseCreatedEvent) {
      Databases.Clear();
      foreach (IDatabase database in databaseManager.GetAllDatabases()) {
        Databases.Add(database);
      }
    }
  }
}
