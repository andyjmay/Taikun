﻿using System.Collections.Generic;

namespace Taikun {
  public interface IDatabase<T> where T : IDatabaseTable {
    string Name { get; }
    string Description { get; set; }

    bool DatabaseTableExists(string tableName);
    T CreateDatabaseTable(T databaseTable);
    T GetDatabaseTable(string tableName, bool loadData);
    IEnumerable<T> GetDatabaseTables();
    void DeleteDatabaseTable(T databaseTable);
  }
}