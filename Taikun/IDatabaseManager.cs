using System.Collections.Generic;

namespace Taikun {
  public interface IDatabaseManager<T> {
    bool DatabaseExists(string databaseName);
    IEnumerable<T> GetAllDatabases();
    T GetDatabase(string databaseName);
    T CreateDatabase(T database);
    T UpdateDatabase(T database);
    void DeleteDatabase(T database);

    string GetDatabaseConnectionString(string databaseName);
  }
}