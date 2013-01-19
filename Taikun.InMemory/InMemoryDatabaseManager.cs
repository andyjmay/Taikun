using System;
using System.Collections.Generic;
using System.Linq;

namespace Taikun.InMemory {
  public class InMemoryDatabaseManager : IDatabaseManager<InMemoryDatabase> {
    private readonly List<InMemoryDatabase> databases;
 
    public InMemoryDatabaseManager() {
      databases = new List<InMemoryDatabase>();
    }

    public bool DatabaseExists(string name) {
      return databases.Any(d => d.Name == name);
    }

    public IEnumerable<InMemoryDatabase> GetAllDatabases() {
      return databases;
    }

    public InMemoryDatabase GetDatabase(string name) {
      return databases.Single(d => d.Name == name);
    }

    public InMemoryDatabase AddExistingDatabase(string name, string description) {
      var database = new InMemoryDatabase(name) { Description = description };
      databases.Add(database);
      return database;
    }

    public InMemoryDatabase CreateDatabase(string name, string description) {
      var database = new InMemoryDatabase(name) {Description = description};
      databases.Add(database);
      return database;
    }

    public void UpdateDatabaseDescription(string name, string description) {
      var database = GetDatabase(name);
      database.Description = description;
    }

    public void DeleteDatabase(string name) {
      var database = GetDatabase(name);
      databases.Remove(database);
    }

    public string GetDatabaseConnectionString(string name) {
      throw new NotImplementedException();
    }
  }
}
