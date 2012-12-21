using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Data;
using Taikun.SqlServer;

namespace Taikun.Demo.WPF.ViewModels {
  public class DatabaseTablesViewModel : ViewModelBase {
    readonly IDatabaseManager databaseManager;
    
    private IDatabaseTable selectedTable;
    public IDatabaseTable SelectedTable {
      get { return selectedTable; }
      set {
        selectedTable = value;
        RaisePropertyChanged(() => SelectedTable);
        Messenger.Default.Send(new Events.DatabaseTableSelected(SelectedTable));
      }
    }

    private IDatabase selectedDatabase;
    public IDatabase SelectedDatabase {
      get { return selectedDatabase; }
      set {
        selectedDatabase = value;
        RaisePropertyChanged(() => SelectedDatabase);
      }
    }

    private bool creatingNewTable;
    public bool CreatingNewTable {
      get { return creatingNewTable; }
      set {
        creatingNewTable = value;
        RaisePropertyChanged(() => CreatingNewTable);
      }
    }

    public ObservableCollection<IDatabaseTable> Tables { get; private set; }
    
    public RelayCommand NewTable { get; private set; }
    public RelayCommand CancelCreateNewTable { get; private set; }
    public RelayCommand CreateNewTable { get; private set; }
    
    public DatabaseTablesViewModel(IDatabaseManager databaseManager) {
      this.databaseManager = databaseManager;
      CreatingNewTable = false;
      if (!IsInDesignMode) {
        Tables = new ObservableCollection<IDatabaseTable>();
      } else {
        Tables = new ObservableCollection<IDatabaseTable> {
          new SqlServerDatabaseTable(new DataTable("Table 1"))
        };
      }
      
      NewTable = new RelayCommand(addNewTable, () => { return (SelectedDatabase != null); });
      CancelCreateNewTable = new RelayCommand(cancelCreateNewTable, () => { return (CreatingNewTable); });
      Messenger.Default.Register<Events.DatabaseSelected>(this, databaseSelectedEventHandler);
      Messenger.Default.Register<Events.DatabaseTableCreated>(this, databaseTableCreatedEventHandler);
    }

    private void addNewTable() {
      CreatingNewTable = true;
      SelectedTable = null;
    }

    private void cancelCreateNewTable() {
      CreatingNewTable = false;
    }
    
    private void databaseSelectedEventHandler(Events.DatabaseSelected databaseSelectedEvent) {
      Tables.Clear();
      SelectedDatabase = databaseSelectedEvent.Database;
      foreach (IDatabaseTable databaseTable in databaseManager.GetDatabaseTables(SelectedDatabase)) {
        Tables.Add(databaseTable);
      }
    }

    private void databaseTableCreatedEventHandler(Events.DatabaseTableCreated databaseTableCreatedEvent) {
      CreatingNewTable = false;
      Tables.Add(databaseTableCreatedEvent.DatabaseTable);
      SelectedTable = databaseTableCreatedEvent.DatabaseTable;
    }

    public override void Cleanup() {
      Messenger.Default.Unregister<Events.DatabaseSelected>(this);
      Messenger.Default.Unregister<Events.DatabaseTableCreated>(this);
      base.Cleanup();
    }
  }
}
