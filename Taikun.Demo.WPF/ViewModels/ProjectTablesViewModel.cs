using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Data;
using System.Collections.ObjectModel;
using Taikun.SqlServer;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;

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

    public ObservableCollection<IProjectTable> Tables { get; private set; }

    public RelayCommand LoadTableData { get; private set; }
    
    public ProjectTablesViewModel(IProjectManager projectManager) {
      this.projectManager = projectManager;
      if (!IsInDesignMode) {
        Tables = new ObservableCollection<IProjectTable>();
      } else {
        Tables = new ObservableCollection<IProjectTable> {
          new SqlServerProjectTable(new DataTable("Table 1"))
        };
      }
      LoadTableData = new RelayCommand(loadProjectTableData, () => { return (SelectedTable != null); });
      Messenger.Default.Register<Events.ProjectSelected>(this, projectSelectedEventHandler);
    }

    private void loadProjectTableData() {
      IProjectTable projectTableWithData = projectManager.GetProjectTable(SelectedProject, SelectedTable.Name, true);
      SelectedTableData = ((SqlServerProjectTable)projectTableWithData).Schema;
    }

    private void projectSelectedEventHandler(Events.ProjectSelected projectSelectedEvent) {
      Tables.Clear();
      SelectedProject = projectSelectedEvent.Project;
      foreach (IProjectTable projectTable in projectManager.GetProjectTables(SelectedProject)) {
        Tables.Add(projectTable);
      }
    }

    public override void Cleanup() {
      Messenger.Default.Unregister<Events.ProjectSelected>(this);
      base.Cleanup();
    }
  }
}
