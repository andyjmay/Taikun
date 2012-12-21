using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Taikun.SqlServer;
using GalaSoft.MvvmLight.Messaging;

namespace Taikun.Demo.WPF.ViewModels {
  public class CreateDatabaseViewModel : ViewModelBase {
    private string databaseName;
    public string DatabaseName {
      get { return databaseName; }
      set { 
        databaseName = value;
        RaisePropertyChanged(() => DatabaseName);
      }
    }

    private string databaseDescription;
    public string DatabaseDescription {
      get { return databaseDescription; }
      set {
        databaseDescription = value;
        RaisePropertyChanged(() => DatabaseDescription);
      }
    }
        
    public RelayCommand CreateDatabase { get; private set; }

    private IDatabaseManager databaseManager;

    public CreateDatabaseViewModel(IDatabaseManager databaseManager) {
      this.databaseManager = databaseManager;
      CreateDatabase = new RelayCommand(createDatabase, () => { return (!string.IsNullOrWhiteSpace(DatabaseName)); });
    }

    private void createDatabase() {
      IDatabase database = databaseManager.CreateDatabase(new SqlServerDatabase(databaseManager, DatabaseName) {
        Description = DatabaseDescription
      });
      Messenger.Default.Send<Events.DatabaseCreated>(new Events.DatabaseCreated(database));
    }
  }
}
