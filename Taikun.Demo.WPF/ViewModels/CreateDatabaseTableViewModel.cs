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
  public class CreateDatabaseTableViewModel : ViewModelBase {
    private readonly IDatabaseManager databaseManager;

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

    private IDatabase selectedDatabase;
    public IDatabase SelectedDatabase {
      get { return selectedDatabase; }
      set {
        selectedDatabase = value;
        RaisePropertyChanged(() => SelectedDatabase);
      }
    }

    public ObservableCollection<SqlServerTableColumn> DatabaseColumns { get; private set; }
    
    public RelayCommand AddColumn { get; private set; }
    public RelayCommand RemoveColumn { get; private set; }
    public RelayCommand CreateDatabaseTable { get; private set; }

    public CreateDatabaseTableViewModel(IDatabaseManager databaseManager) {
      this.databaseManager = databaseManager;
      if (!IsInDesignMode) {
        DatabaseColumns = new ObservableCollection<SqlServerTableColumn>();
      } else {
        DatabaseColumns = new ObservableCollection<SqlServerTableColumn> {
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

      AddColumn = new RelayCommand(() => DatabaseColumns.Add(new SqlServerTableColumn()));
      RemoveColumn = new RelayCommand(() => {
        if (SelectedColumn == null) {
          return;
        }
        DatabaseColumns.Remove(SelectedColumn);
      }, () => SelectedColumn != null);
      CreateDatabaseTable = new RelayCommand(createDatabaseTable, () => !string.IsNullOrWhiteSpace(TableName));
      Messenger.Default.Register<Events.DatabaseSelected>(this, selected => {
        SelectedDatabase = selected.Database;
      });
      
    }

    private void createDatabaseTable() {
      var dataTable = new DataTable(TableName);
      var primaryKeyColumns = new List<DataColumn>();
      foreach (SqlServerTableColumn sqlServerTableColumn in DatabaseColumns) {
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
      var databaseTable = new SqlServerDatabaseTable(dataTable);
      SelectedDatabase.CreateDatabaseTable(databaseTable);
      Messenger.Default.Send(new Events.DatabaseTableCreated(databaseTable));
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
