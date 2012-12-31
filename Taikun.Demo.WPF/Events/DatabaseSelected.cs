using Taikun.SqlServer;

namespace Taikun.Demo.WPF.Events {
  public class DatabaseSelected {
    public DatabaseSelected(SqlServerDatabase database) {
      Database = database;
    }
      
    public SqlServerDatabase Database { get; private set; }
  }
}
