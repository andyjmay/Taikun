﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace Taikun.SqlServer.Tests {
  [TestClass]
  public class SqlServerDatabaseManagerTests {
    private const string dataSource = @"(local)\SQLEXPRESS";
    private const string database = "Taikun";
    private readonly SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", dataSource, database));

    [TestMethod]
    public void GetAllDatabases_GetsAllDatabases() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      var createdDatabases = new List<IDatabase>();
      for (int i = 0; i < 10; i++) {
        createdDatabases.Add(databaseManager.CreateDatabase(getRandomDatabase()));
      }
      IEnumerable<IDatabase> databases = databaseManager.GetAllDatabases();
      Assert.IsTrue(databases.Any());
      deleteAllDatabases();
    }
    
    [TestMethod]
    public void GetDatabase_GetsDatabase() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      var createdDatabases = new List<IDatabase>();
      for (int i = 0; i < 10; i++) {
        createdDatabases.Add(databaseManager.CreateDatabase(getRandomDatabase()));
      }
      IDatabase databaseToFind = createdDatabases[new Random().Next(0, createdDatabases.Count - 1)];
      IDatabase databaseFound = databaseManager.GetDatabase(databaseToFind.Name);
      Assert.AreEqual(databaseToFind.Name, databaseFound.Name);
      Assert.AreEqual(databaseToFind.Description, databaseFound.Description);
      deleteAllDatabases();
    }

    [TestMethod]
    public void CreateDatabase_CreatesDatabase() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      IDatabase createdDatabase = databaseManager.CreateDatabase(getRandomDatabase());
      Assert.IsNotNull(createdDatabase);
      Assert.IsTrue(databaseExists(createdDatabase.Name));
      deleteAllDatabases();
    }

    [TestMethod]
    public void UpdateDatabase_UpdatesDatabase() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      IDatabase createdDatabase = databaseManager.CreateDatabase(getRandomDatabase());
      string newDescription = DateTime.Now.ToFileTimeUtc().ToString();
      createdDatabase.Description = newDescription;
      databaseManager.UpdateDatabase(createdDatabase);

      IDatabase updatedDatabase = databaseManager.GetDatabase(createdDatabase.Name);
      databaseManager.DeleteDatabase(createdDatabase);
      Assert.AreEqual(createdDatabase.Description, updatedDatabase.Description);
      deleteAllDatabases();
    }

    [TestMethod]
    public void DeleteDatabase_DeletesDatabase() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      IDatabase createdDatabase = databaseManager.CreateDatabase(getRandomDatabase());
      databaseManager.DeleteDatabase(createdDatabase);
      Assert.IsFalse(databaseExists(createdDatabase.Name));
    }

    private void deleteAllDatabases() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      foreach (IDatabase database in databaseManager.GetAllDatabases()) {
        databaseManager.DeleteDatabase(database);
      }
    }

    private IDatabase getRandomDatabase() {
      return new SqlServerDatabase {
        Name = Guid.NewGuid().ToString(),
        Description = DateTime.Now.ToFileTimeUtc().ToString()
      };
    }

    private bool databaseExists(string databaseName) {
      using (var connection = new SqlConnection(getMasterDatabaseConnectionString())) {
        string queryString = "SELECT Count(*) FROM sys.databases WHERE [name] = '" + databaseName + "'";
        using (var command = new SqlCommand(queryString, connection)) {
          connection.Open();
          try {
            int result = (int)command.ExecuteScalar();
            return result > 0;
          } catch (SqlException ex) {
            Console.WriteLine(ex);
            return false;
          }
        }
      }
    }

    private string getMasterDatabaseConnectionString() {
      var builder = new SqlConnectionStringBuilder(connectionStringBuilder.ConnectionString);
      builder.InitialCatalog = "master";
      return builder.ConnectionString;
    }
  }
}