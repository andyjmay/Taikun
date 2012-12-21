using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Taikun.Demo.WPF.ViewModels {
  public class MainViewModel : ViewModelBase {
    private bool databaseIsSelected;
    public bool DatabaseIsSelected {
      get { return databaseIsSelected; }
      set {
        databaseIsSelected = value;
        RaisePropertyChanged(() => DatabaseIsSelected);
      }
    }

    public MainViewModel() {
      DatabaseIsSelected = false;
      Messenger.Default.Register<Events.DatabaseSelected>(this, databaseSelectedEventHandler);
    }

    private void databaseSelectedEventHandler(Events.DatabaseSelected databaseSelectedEvent) {
      DatabaseIsSelected = true;
    }
  }
}
