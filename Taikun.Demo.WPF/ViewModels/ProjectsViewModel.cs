using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Taikun.SqlServer;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Taikun.Demo.WPF.ViewModels {
  public class DatabasesViewModel : ViewModelBase {
    private readonly IDatabaseManager<SqlServerDatabase> databaseManager;

    public ObservableCollection<SqlServerDatabase> Databases { get; private set; }

    public RelayCommand<SqlServerDatabase> SelectDatabase { get; private set; } 

    public DatabasesViewModel(IDatabaseManager<SqlServerDatabase> databaseManager) {
      this.databaseManager = databaseManager;
      SelectDatabase = new RelayCommand<SqlServerDatabase>((database) => {
        var databaseSelected = new Events.DatabaseSelected(database);
        Messenger.Default.Send<Events.DatabaseSelected>(databaseSelected);
      });

      Databases = new ObservableCollection<SqlServerDatabase>(databaseManager.GetAllDatabases());
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
