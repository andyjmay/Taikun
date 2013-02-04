using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Taikun.SqlServer;

namespace Taikun.Demo.Web.Controllers {
  public class DatabasesController : ApiController {
    private readonly SqlServerDatabaseManager databaseManager;

    public DatabasesController(SqlServerDatabaseManager databaseManager) {
      this.databaseManager = databaseManager;
    }

    public IEnumerable<Models.Database> Get() {
      var databases = databaseManager.GetAllDatabases();
      return databases.Select(database => new Models.Database() { Name = database.Name, Description = database.Description }).ToList();
    }

    public Models.Database Get(string name) {
      var database = databaseManager.GetDatabase(name);
      return new Models.Database { Name = database.Name, Description = database.Description };
    }

    public void Post([FromBody]string name, string description) {
      databaseManager.CreateDatabase(name, description);
    }

    public void Put([FromBody]string name, [FromBody]string description) {
      databaseManager.UpdateDatabaseDescription(name, description);
    }

    public void Delete([FromBody]string name) {
      databaseManager.DeleteDatabase(name);
    }
  }
}