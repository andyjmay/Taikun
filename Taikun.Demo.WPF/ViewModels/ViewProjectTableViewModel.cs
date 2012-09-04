using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Data;
using Taikun.SqlServer;

namespace Taikun.Demo.WPF.ViewModels {
  public class ViewProjectTableViewModel : ViewModelBase {
    private readonly IProjectManager projectManager;

    private IProject selectedProject;
    public IProject SelectedProject {
      get { return selectedProject; }
      set {
        selectedProject = value;
        RaisePropertyChanged(() => SelectedProject);
      }
    }

    private IProjectTable selectedProjectTable;
    public IProjectTable SelectedProjectTable {
      get { return selectedProjectTable; }
      set {
        selectedProjectTable = value;
        RaisePropertyChanged(() => SelectedProjectTable);
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

    public ViewProjectTableViewModel(IProjectManager projectManager) {
      this.projectManager = projectManager;

      LoadTableData = new RelayCommand(loadProjectTableData, () => SelectedProjectTable != null);
      Messenger.Default.Register<Events.ProjectSelected>(this, projectSelectedEventHandler);
      Messenger.Default.Register<Events.ProjectTableSelected>(this, projectTableSelectedEventHandler);
    }
    
    private void projectSelectedEventHandler(Events.ProjectSelected projectSelectedEvent) {
      SelectedProject = projectSelectedEvent.Project;
      SelectedProjectTable = null;
      SelectedTableData = null;
    }

    private void projectTableSelectedEventHandler(Events.ProjectTableSelected projectTableSelectedEvent) {
      SelectedProjectTable = projectManager.GetProjectTable(SelectedProject, projectTableSelectedEvent.ProjectTable.Name, loadData: false);
      SelectedTableData = null;
    }
    
    private void loadProjectTableData() {
      IProjectTable projectTableWithData = projectManager.GetProjectTable(SelectedProject, SelectedProjectTable.Name, loadData: true);
      SelectedTableData = ((SqlServerProjectTable)projectTableWithData).Schema;
    }

    public override void Cleanup() {
      Messenger.Default.Unregister<Events.ProjectSelected>(this);
      Messenger.Default.Unregister<Events.ProjectTableSelected>(this);
      base.Cleanup();
    }
  }
}
