using System.Collections.Generic;

namespace Taikun {
  public interface IDatabase {
    string Name { get; set; }
    string Description { get; set; }
    string ConnectionString { get; }

    bool DatabaseTableExists(string tableName);
    void CreateDatabaseTable(IDatabaseTable databaseTable);
    IDatabaseTable GetDatabaseTable(string tableName, bool loadData);
    IEnumerable<IDatabaseTable> GetDatabaseTables();
    void DeleteDatabaseTable(IDatabaseTable databaseTable);
  }
}