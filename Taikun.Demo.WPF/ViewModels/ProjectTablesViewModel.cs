using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Data;
using Taikun.SqlServer;

namespace Taikun.Demo.WPF.ViewModels {
  public class ProjectTablesViewModel : ViewModelBase {
    IProjectManager projectManager;
    
    private DataTable selectedTableData;
    public DataTable SelectedTableData {
      get { return selectedTableData; }
      set {
        selectedTableData = value;
        RaisePropertyChanged(() => SelectedTableData);
      }
    }

    private IProjectTable selectedTable;
    public IProjectTable SelectedTable {
      get { return selectedTable; }
      set {
        selectedTable = value;
        RaisePropertyChanged(() => SelectedTable);
        if (SelectedTable is SqlServerProjectTable) {
          SelectedTableData = ((SqlServerProjectTable)SelectedTable).Schema;
        }
      }
    }

    private IProject selectedProject;
    public IProject SelectedProject {
      get { return selectedProject; }
      set {
        selectedProject = value;
        RaisePropertyChanged(() => SelectedProject);
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
    
    private string newTableName;
    public string NewTableName {
      get { return newTableName; }
      set {
        newTableName = value;
        RaisePropertyChanged(() => NewTableName);
      }
    }

    public ObservableCollection<IProjectTable> Tables { get; private set; }

    public RelayCommand LoadTableData { get; private set; }
    public RelayCommand NewTable { get; private set; }
    public RelayCommand CancelCreateNewTable { get; private set; }
    public RelayCommand CreateNewTable { get; private set; }
    
    public ProjectTablesViewModel(IProjectManager projectManager) {
      this.projectManager = projectManager;
      CreatingNewTable = false;
      if (!IsInDesignMode) {
        Tables = new ObservableCollection<IProjectTable>();
      } else {
        Tables = new ObservableCollection<IProjectTable> {
          new SqlServerProjectTable(new DataTable("Table 1"))
        };
      }
      LoadTableData = new RelayCommand(loadProjectTableData, () => { return (SelectedTable != null); });
      NewTable = new RelayCommand(addNewTable, () => { return (SelectedProject != null); });
      CancelCreateNewTable = new RelayCommand(cancelCreateNewTable, () => { return (CreatingNewTable); });
      Messenger.Default.Register<Events.ProjectSelected>(this, projectSelectedEventHandler);
      Messenger.Default.Register<Events.ProjectTableCreated>(this, projectTableCreatedEventHandler);
    }

    private void loadProjectTableData() {
      IProjectTable projectTableWithData = projectManager.GetProjectTable(SelectedProject, SelectedTable.Name, true);
      SelectedTableData = ((SqlServerProjectTable)projectTableWithData).Schema;
    }

    private void addNewTable() {
      CreatingNewTable = true;
      SelectedTable = null;
    }

    private void cancelCreateNewTable() {
      CreatingNewTable = false;
      NewTableName = string.Empty;
    }
    

    private void projectSelectedEventHandler(Events.ProjectSelected projectSelectedEvent) {
      Tables.Clear();
      SelectedProject = projectSelectedEvent.Project;
      foreach (IProjectTable projectTable in projectManager.GetProjectTables(SelectedProject)) {
        Tables.Add(projectTable);
      }
    }

    private void projectTableCreatedEventHandler(Events.ProjectTableCreated projectTableCreatedEvent) {
      CreatingNewTable = false;
      Tables.Add(projectTableCreatedEvent.ProjectTable);
      SelectedTable = projectTableCreatedEvent.ProjectTable;
    }

    public override void Cleanup() {
      Messenger.Default.Unregister<Events.ProjectSelected>(this);
      base.Cleanup();
    }
  }
}
