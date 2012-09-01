using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Taikun.Demo.WPF.ViewModels {
  public class MainViewModel : ViewModelBase {
    private bool projectIsSelected;
    public bool ProjectIsSelected {
      get { return projectIsSelected; }
      set {
        projectIsSelected = value;
        RaisePropertyChanged(() => ProjectIsSelected);
      }
    }

    public MainViewModel() {
      ProjectIsSelected = false;
      Messenger.Default.Register<Events.ProjectSelected>(this, projectSelectedEventHandler);
    }

    private void projectSelectedEventHandler(Events.ProjectSelected projectSelectedEvent) {
      ProjectIsSelected = true;
    }
  }
}
