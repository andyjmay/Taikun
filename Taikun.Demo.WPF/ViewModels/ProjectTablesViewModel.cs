using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Data;
using Taikun.SqlServer;

namespace Taikun.Demo.WPF.ViewModels {
  public class ProjectTablesViewModel : ViewModelBase {
    readonly IProjectManager projectManager;
    
    private IProjectTable selectedTable;
    public IProjectTable SelectedTable {
      get { return selectedTable; }
      set {
        selectedTable = value;
        RaisePropertyChanged(() => SelectedTable);
        Messenger.Default.Send(new Events.ProjectTableSelected(SelectedTable));
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
      
      NewTable = new RelayCommand(addNewTable, () => { return (SelectedProject != null); });
      CancelCreateNewTable = new RelayCommand(cancelCreateNewTable, () => { return (CreatingNewTable); });
      Messenger.Default.Register<Events.ProjectSelected>(this, projectSelectedEventHandler);
      Messenger.Default.Register<Events.ProjectTableCreated>(this, projectTableCreatedEventHandler);
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
      Messenger.Default.Unregister<Events.ProjectTableCreated>(this);
      base.Cleanup();
    }
  }
}
