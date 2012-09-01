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
      LoadTableData = new RelayCommand(() => {
        int i = 0;
      });
      Messenger.Default.Register<Events.ProjectSelected>(this, projectSelectedEventHandler);
    }

    private void projectSelectedEventHandler(Events.ProjectSelected projectSelectedEvent) {
      Tables.Clear();
      foreach (IProjectTable projectTable in projectManager.GetProjectTables(projectSelectedEvent.Project)) {
        Tables.Add(projectTable);
      }
    }

    public override void Cleanup() {
      Messenger.Default.Unregister<Events.ProjectSelected>(this);
      base.Cleanup();
    }
  }
}
