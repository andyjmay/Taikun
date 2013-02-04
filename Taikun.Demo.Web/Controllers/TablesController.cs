using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using Taikun.SqlServer;

namespace Taikun.Demo.Web.Controllers {
  public class TablesController : ApiController {
    private readonly SqlServerDatabaseManager databaseManager;

    public TablesController(SqlServerDatabaseManager databaseManager) {
      this.databaseManager = databaseManager;
    }

    public IEnumerable<Models.Table> Get(string name) {
      var database = databaseManager.GetDatabase(name);
      var databaseTables = database.GetDatabaseTables();
      return databaseTables.Select(t => new Models.Table { Name = t.Name }).ToArray();
    }

    public IEnumerable<Models.Column> Get(string name, string tableName) {
      var database = databaseManager.GetDatabase(name);
      SqlServerDatabaseTable table = database.GetDatabaseTable(tableName);
      var columns = new List<Models.Column>();
      foreach (DataColumn column in table.Schema.Columns) {
        columns.Add(new Models.Column {
                                         ColumnName = column.ColumnName,
                                         DataType = column.DataType.ToString(),
                                         MaxLength = column.MaxLength != -1 ? column.MaxLength.ToString() : string.Empty
                                      });
      }
      //string[] primaryKeyColumns = table.Schema.PrimaryKey.Select(s => s.ColumnName).ToArray();
      //var tableDetail = new Models.TableDetail {
      //  PrimaryKeys = primaryKeyColumns,
      //  Columns = columns
      //};
      //return tableDetail;
      return columns;
    }
  }
}
