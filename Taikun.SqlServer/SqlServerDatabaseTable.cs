using System.Data;

namespace Taikun.SqlServer {
  public class SqlServerDatabaseTable : IDatabaseTable {
    public string Name { get { return Schema.TableName; } }
    public DataTable Schema { get; private set; }

    public SqlServerDatabaseTable(DataTable schema) {
      this.Schema = schema;
    }
  }
}
