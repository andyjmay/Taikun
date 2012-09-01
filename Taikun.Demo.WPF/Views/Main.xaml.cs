using GalaSoft.MvvmLight.Messaging;

namespace Taikun.Demo.WPF.Views {
  public partial class Main {
    public Main() {
      Messenger.Default.Register<Events.ProjectSelected>(this, projectSelectedEventHandler);
      Messenger.Default.Register<Events.ProjectCreated>(this, projectCreatedEventHandler);
    }

    private void projectSelectedEventHandler(Events.ProjectSelected projectSelectedEvent) {
      TablesTab.IsSelected = true;
      this.Title = "Taikun Demo - " + projectSelectedEvent.Project.DatabaseName;
    }

    private void projectCreatedEventHandler(Events.ProjectCreated projectCreatedEvent) {
      ProjectsTab.IsSelected = true;
    }
  }
}
