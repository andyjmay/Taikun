using GalaSoft.MvvmLight.Messaging;

namespace Taikun.Demo.WPF.Views {
  public partial class Main {
    public Main() {
      Messenger.Default.Register<Events.DatabaseSelected>(this, databaseSelectedEventHandler);
      Messenger.Default.Register<Events.DatabaseCreated>(this, databaseCreatedEventHandler);
    }

    private void databaseSelectedEventHandler(Events.DatabaseSelected databaseSelectedEvent) {
      TablesTab.IsSelected = true;
      this.Title = "Taikun Demo - " + databaseSelectedEvent.Database.Name;
    }

    private void databaseCreatedEventHandler(Events.DatabaseCreated databaseCreatedEvent) {
      DatabasesTab.IsSelected = true;
    }
  }
}
