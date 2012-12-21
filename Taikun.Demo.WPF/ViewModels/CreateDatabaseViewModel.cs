﻿using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using Taikun.Demo.WPF.Models;
using Taikun.SqlServer;
using System.Data;
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
      IDatabase database = databaseManager.CreateDatabase(new SqlServerDatabase {
        DatabaseName = DatabaseName,
        Description = DatabaseDescription
      });
      Messenger.Default.Send<Events.DatabaseCreated>(new Events.DatabaseCreated(database));
    }
  }
}
