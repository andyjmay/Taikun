namespace Taikun.Demo.WPF.Events {
  public class DatabaseTableSelected {
    public DatabaseTableSelected(IDatabaseTable databaseTable) {
      DatabaseTable = databaseTable;
    }

    public IDatabaseTable DatabaseTable { get; private set; }
  }
}
