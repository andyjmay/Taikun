using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Taikun.Demo.WPF.Models;
using Taikun.SqlServer;

namespace Taikun.Demo.WPF.ViewModels {
  public class CreateProjectTableViewModel : ViewModelBase {
    private readonly IProjectManager projectManager;

    private string tableName;
    public string TableName {
      get { return tableName; }
      set { 
        tableName = value;
        RaisePropertyChanged(() => TableName);
      }
    }

    private SqlServerTableColumn selectedColumn;
    public SqlServerTableColumn SelectedColumn {
      get { return selectedColumn; }
      set {
        selectedColumn = value;
        RaisePropertyChanged(() => SelectedColumn);
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

    public ObservableCollection<SqlServerTableColumn> ProjectColumns { get; private set; }
    
    public RelayCommand AddColumn { get; private set; }
    public RelayCommand RemoveColumn { get; private set; }
    public RelayCommand CreateProjectTable { get; private set; }

    public CreateProjectTableViewModel(IProjectManager projectManager) {
      this.projectManager = projectManager;
      if (!IsInDesignMode) {
        ProjectColumns = new ObservableCollection<SqlServerTableColumn>();
      } else {
        ProjectColumns = new ObservableCollection<SqlServerTableColumn> {
          new SqlServerTableColumn {
            Name = "ID",
            Type = "int"
          },
          new SqlServerTableColumn {
            Name = "FirstName",
            Type = "string",
            Length = 255
          },
          new SqlServerTableColumn {
            Name = "LastName",
            Type = "string",
            Length = 255
          },
          new SqlServerTableColumn {
            Name = "State",
            Type = "string",
            Length = 2
          },
          new SqlServerTableColumn {
            Name = "Description",
            Type = "string",
            Length = int.MaxValue
          }
        };
      }

      AddColumn = new RelayCommand(() => ProjectColumns.Add(new SqlServerTableColumn()));
      RemoveColumn = new RelayCommand(() => {
        if (SelectedColumn == null) {
          return;
        }
        ProjectColumns.Remove(SelectedColumn);
      }, () => SelectedColumn != null);
      CreateProjectTable = new RelayCommand(createProjectTable, () => !string.IsNullOrWhiteSpace(TableName));
      Messenger.Default.Register<Events.ProjectSelected>(this, selected => {
        SelectedProject = selected.Project;
      });
      
    }

    private void createProjectTable() {
      var dataTable = new DataTable(TableName);
      var primaryKeyColumns = new List<DataColumn>();
      foreach (SqlServerTableColumn sqlServerTableColumn in ProjectColumns) {
        var dataColumn = new DataColumn(sqlServerTableColumn.Name, getTypeForColumn(sqlServerTableColumn));
        dataColumn.AllowDBNull = sqlServerTableColumn.AllowNulls;
        if (sqlServerTableColumn.Length.HasValue) {
          dataColumn.MaxLength = sqlServerTableColumn.Length.Value;
        }
        dataTable.Columns.Add(dataColumn);
        if (sqlServerTableColumn.PrimaryKey) {
          primaryKeyColumns.Add(dataColumn);
        }
        if (sqlServerTableColumn.Identity) {
          dataColumn.AutoIncrement = true;
          dataColumn.AutoIncrementSeed = 1;
          dataColumn.AutoIncrementStep = 1;
        }
      }
      dataTable.PrimaryKey = primaryKeyColumns.ToArray();
      var projectTable = new SqlServerProjectTable(dataTable);
      projectManager.CreateProjectTable(SelectedProject, projectTable);
      Messenger.Default.Send(new Events.ProjectTableCreated(projectTable));
    }

    private Type getTypeForColumn(SqlServerTableColumn sqlServerTableColumn) {
      switch (sqlServerTableColumn.Type) {
        case "string":
          return typeof(string);
        case "int":
          return typeof (int);
        default:
          return null;
      }
    }
  }
}
