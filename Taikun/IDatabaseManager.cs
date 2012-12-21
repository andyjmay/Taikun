using System.Collections.Generic;

namespace Taikun {
  public interface IDatabaseManager {
    bool DatabaseExists(string databaseName);
    IEnumerable<IDatabase> GetAllDatabases();
    IDatabase GetDatabase(string databaseName);
    IDatabase CreateDatabase(IDatabase database);    
    IDatabase UpdateDatabase(IDatabase database);
    void DeleteDatabase(IDatabase database);

    string GetDatabaseConnectionString(string databaseName);
  }
}