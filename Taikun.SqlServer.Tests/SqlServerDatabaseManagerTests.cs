using System;
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
      var createdDatabases = new List<SqlServerDatabase>();
      for (int i = 0; i < 10; i++) {
        createdDatabases.Add(databaseManager.CreateDatabase(getRandomDatabase(databaseManager)));
      }
      IEnumerable<SqlServerDatabase> databases = databaseManager.GetAllDatabases();
      Assert.IsTrue(databases.Any());
      deleteAllDatabases();
    }
    
    [TestMethod]
    public void GetDatabase_GetsDatabase() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      var createdDatabases = new List<SqlServerDatabase>();
      for (int i = 0; i < 10; i++) {
        createdDatabases.Add(databaseManager.CreateDatabase(getRandomDatabase(databaseManager)));
      }
      SqlServerDatabase databaseToFind = createdDatabases[new Random().Next(0, createdDatabases.Count - 1)];
      SqlServerDatabase databaseFound = databaseManager.GetDatabase(databaseToFind.Name);
      Assert.AreEqual(databaseToFind.Name, databaseFound.Name);
      Assert.AreEqual(databaseToFind.Description, databaseFound.Description);
      deleteAllDatabases();
    }

    [TestMethod]
    public void CreateDatabase_CreatesDatabase() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      SqlServerDatabase createdDatabase = databaseManager.CreateDatabase(getRandomDatabase(databaseManager));
      Assert.IsNotNull(createdDatabase);
      Assert.IsTrue(databaseExists(createdDatabase.Name));
      deleteAllDatabases();
    }

    [TestMethod]
    public void UpdateDatabase_UpdatesDatabase() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      SqlServerDatabase createdDatabase = databaseManager.CreateDatabase(getRandomDatabase(databaseManager));
      string newDescription = DateTime.Now.ToFileTimeUtc().ToString();
      createdDatabase.Description = newDescription;
      databaseManager.UpdateDatabase(createdDatabase);

      SqlServerDatabase updatedDatabase = databaseManager.GetDatabase(createdDatabase.Name);
      databaseManager.DeleteDatabase(createdDatabase);
      Assert.AreEqual(createdDatabase.Description, updatedDatabase.Description);
      deleteAllDatabases();
    }

    [TestMethod]
    public void DeleteDatabase_DeletesDatabase() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      SqlServerDatabase createdDatabase = databaseManager.CreateDatabase(getRandomDatabase(databaseManager));
      databaseManager.DeleteDatabase(createdDatabase);
      Assert.IsFalse(databaseExists(createdDatabase.Name));
    }

    private void deleteAllDatabases() {
      var databaseManager = new SqlServerDatabaseManager(connectionStringBuilder.ConnectionString, true);
      foreach (SqlServerDatabase database in databaseManager.GetAllDatabases()) {
        databaseManager.DeleteDatabase(database);
      }
    }

    private SqlServerDatabase getRandomDatabase(IDatabaseManager<SqlServerDatabase> databaseManager) {
      string databaseName = Guid.NewGuid().ToString();
      return new SqlServerDatabase(databaseManager, databaseName) {
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

    private string GetDatabaseConnectionString(string databaseName) {
      var builder = new SqlConnectionStringBuilder(connectionStringBuilder.ConnectionString);
      builder.InitialCatalog = databaseName;
      return builder.ConnectionString;
    }
  }
}
