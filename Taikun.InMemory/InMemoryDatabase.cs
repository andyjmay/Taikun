using System.Collections.Generic;
using System.Linq;

namespace Taikun.InMemory {
  public class InMemoryDatabase : IDatabase<InMemoryDatabaseTable> {
    private readonly List<InMemoryDatabaseTable> tables;
    
    public string Name { get; private set; }
    public string Description { get; set; }

    public InMemoryDatabase(string name) {
      this.Name = name;
      this.tables = new List<InMemoryDatabaseTable>();
    }

    public bool DatabaseTableExists(string tableName) {
      return tables.Any(d => d.Name == tableName);
    }

    public InMemoryDatabaseTable CreateDatabaseTable(InMemoryDatabaseTable databaseTable) {
      tables.Add(databaseTable);
      return databaseTable;
    }

    public InMemoryDatabaseTable GetDatabaseTable(string tableName) {
      return tables.Single(t => t.Name == tableName);
    }

    public IEnumerable<InMemoryDatabaseTable> GetDatabaseTables() {
      return tables;
    }

    public void DeleteDatabaseTable(string tableName) {
      tables.Remove(GetDatabaseTable(tableName));
    }
  }
}
