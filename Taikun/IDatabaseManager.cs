﻿using System.Collections.Generic;

namespace Taikun {
  public interface IDatabaseManager<out T, T1> where T : IDatabase<T1> where T1 : IDatabaseTable {
    bool DatabaseExists(string name);
    IEnumerable<T> GetAllDatabases();
    T GetDatabase(string name);
    T AddExistingDatabase(string name, string description);
    T CreateDatabase(string name, string description);
    void UpdateDatabaseDescription(string name, string description);
    void DeleteDatabase(string name);

    string GetDatabaseConnectionString(string name);
  }
}