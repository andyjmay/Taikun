using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using Taikun.SqlServer;

namespace Taikun.Demo.Web.Controllers {
  public class DatabasesController : ApiController {
    private readonly IDatabaseManager<SqlServerDatabase> databaseManager;

    public DatabasesController() {
      databaseManager = new SqlServerDatabaseManager(ConfigurationManager.ConnectionStrings["Taikun"].ConnectionString, createIfNotExists: true);
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