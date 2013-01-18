using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using Taikun.SqlServer;

namespace Taikun.Demo.Web.Controllers {
  public class TablesController : ApiController {
    private readonly IDatabaseManager<SqlServerDatabase> databaseManager;

    public TablesController() {
      databaseManager = new SqlServerDatabaseManager(ConfigurationManager.ConnectionStrings["Taikun"].ConnectionString, createIfNotExists: true);
    }
    
    // GET api/tables/5
    public IEnumerable<Models.Table> Get(string name) {
      var database = databaseManager.GetDatabase(name);
      var databaseTables = database.GetDatabaseTables();
      return databaseTables.Select(t => new Models.Table { Name = t.Name }).ToArray();
    }
  }
}
