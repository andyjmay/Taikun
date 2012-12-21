using Taikun.SqlServer;
using Taikun.Demo.WPF.Models;
namespace Taikun.Demo.WPF.Events {
  public class DatabaseSelected {
    public DatabaseSelected(IDatabase database) {
      Database = database;
    }
      
    public IDatabase Database { get; private set; }
  }
}
