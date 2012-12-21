using System.Collections.Generic;

namespace Taikun {
  public interface IDatabaseManager {
    bool DatabaseExists(string databaseName);
    IEnumerable<IDatabase> GetAllDatabases();
    IDatabase GetDatabase(string databaseName);
    IDatabase CreateDatabase(IDatabase database);    
    IDatabase UpdateDatabase(IDatabase database);
    void DeleteDatabase(IDatabase database);

    bool DatabaseTableExists(IDatabase database, string tableName);
    void CreateDatabaseTable(IDatabase database, IDatabaseTable databaseTable);
    IDatabaseTable GetDatabaseTable(IDatabase database, string tableName, bool loadData);
    IEnumerable<IDatabaseTable> GetDatabaseTables(IDatabase database);
    void DeleteDatabaseTable(IDatabase database, IDatabaseTable databaseTable);
  }
}