using Taikun.SqlServer;

namespace Taikun.Demo.WPF.Events {
  public class DatabaseCreated {
    public DatabaseCreated(SqlServerDatabase database) {
      Database = database;
    }
    public SqlServerDatabase Database { get; private set; }
  }
}
