namespace Taikun.Demo.WPF.Events {
  public class DatabaseCreated {
    public DatabaseCreated(IDatabase database) {
      Database = database;
    }
    public IDatabase Database { get; private set; }
  }
}
