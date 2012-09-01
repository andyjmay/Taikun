using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;

namespace Taikun.Demo.WPF.Views {
  public partial class Main {
    public Main() {
      Messenger.Default.Register<Events.ProjectSelected>(this, projectSelectedEventHandler);
    }

    private void projectSelectedEventHandler(Events.ProjectSelected projectSelectedEvent) {
      TablesTab.IsSelected = true;
    }
  }
}
