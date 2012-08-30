using System.Data;

namespace Taikun.SqlServer {
  public class SqlServerProjectTable : IProjectTable {
    public string Name { get { return Schema.TableName; } }
    public DataTable Schema { get; private set; }

    public SqlServerProjectTable(DataTable schema) {
      this.Schema = schema;
    }
  }
}
