using System.Collections.Generic;

namespace Taikun {
  public interface IDatabase {
    string Name { get; }
    string Description { get; set; }

    bool DatabaseTableExists(string tableName);
    void CreateDatabaseTable(IDatabaseTable databaseTable);
    IDatabaseTable GetDatabaseTable(string tableName, bool loadData);
    IEnumerable<IDatabaseTable> GetDatabaseTables();
    void DeleteDatabaseTable(IDatabaseTable databaseTable);
  }
}