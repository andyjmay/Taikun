using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Data;
using Taikun.SqlServer;

namespace Taikun.Demo.WPF.ViewModels {
  public class ViewDatabaseTableViewModel : ViewModelBase {
    private SqlServerDatabase selectedDatabase;
    public SqlServerDatabase SelectedDatabase {
      get { return selectedDatabase; }
      set {
        selectedDatabase = value;
        RaisePropertyChanged(() => SelectedDatabase);
      }
    }

    private IDatabaseTable selectedDatabaseTable;
    public IDatabaseTable SelectedDatabaseTable {
      get { return selectedDatabaseTable; }
      set {
        selectedDatabaseTable = value;
        RaisePropertyChanged(() => SelectedDatabaseTable);
      }
    }

    private DataTable selectedTableData;
    public DataTable SelectedTableData {
      get { return selectedTableData; }
      set {
        selectedTableData = value;
        RaisePropertyChanged(() => SelectedTableData);
      }
    }

    public RelayCommand LoadTableData { get; private set; }

    public ViewDatabaseTableViewModel() {
      LoadTableData = new RelayCommand(loadDatabaseTableData, () => SelectedDatabaseTable != null);
      Messenger.Default.Register<Events.DatabaseSelected>(this, databaseSelectedEventHandler);
      Messenger.Default.Register<Events.DatabaseTableSelected>(this, databaseTableSelectedEventHandler);
    }
    
    private void databaseSelectedEventHandler(Events.DatabaseSelected databaseSelectedEvent) {
      SelectedDatabase = databaseSelectedEvent.Database;
      SelectedDatabaseTable = null;
      SelectedTableData = null;
    }

    private void databaseTableSelectedEventHandler(Events.DatabaseTableSelected databaseTableSelectedEvent) {
      if (databaseTableSelectedEvent.DatabaseTable != null) {
        var table = SelectedDatabase.GetDatabaseTable(databaseTableSelectedEvent.DatabaseTable.Name);
        if (table != null) {
          SelectedDatabaseTable = table;
          SelectedTableData = table.Schema;
        }
      } else {
        SelectedDatabaseTable = null;
        SelectedTableData = null;
      }
    }

    private void loadDatabaseTableData() {
      SelectedTableData = SelectedDatabase.GetDataTable(SelectedDatabaseTable.Name, loadData: true);
    }

    public override void Cleanup() {
      Messenger.Default.Unregister<Events.DatabaseSelected>(this);
      Messenger.Default.Unregister<Events.DatabaseTableSelected>(this);
      base.Cleanup();
    }
  }
}
