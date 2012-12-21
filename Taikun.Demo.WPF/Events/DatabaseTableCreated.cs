namespace Taikun.Demo.WPF.Events {
  public class DatabaseTableCreated {
    public DatabaseTableCreated(IDatabaseTable databaseTable) {
      DatabaseTable = databaseTable;
    }

    public IDatabaseTable DatabaseTable { get; private set; }
  }
}
