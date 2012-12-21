using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Taikun.SqlServer {
  public class SqlServerDatabaseManager : IDatabaseManager {
    private readonly SqlConnectionStringBuilder connectionStringBuilder;

    /// <summary>
    /// Creates a new SQL Server database Manager instance
    /// </summary>
    /// <param name="connectionString">The connection string</param>
    /// <param name="createIfNotExists">If true, the Taikun database is created if it doesn't already exist</param>
    public SqlServerDatabaseManager(string connectionString, bool createIfNotExists = false) {
      connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      if (createIfNotExists && !DatabaseExists(connectionStringBuilder.InitialCatalog)) {
        createDatabase(connectionStringBuilder.InitialCatalog);
        string createTableCommand = "CREATE TABLE [dbo].[Databases]([ID] [int] IDENTITY(1,1) NOT NULL,[Name] [nvarchar](255) NOT NULL,[Description] [nvarchar](255) NULL, CONSTRAINT [PK_Databases] PRIMARY KEY CLUSTERED([ID] ASC))";
        using (var connection = new SqlConnection(GetDatabaseConnectionString(connectionStringBuilder.InitialCatalog))) {
          using (var command = new SqlCommand(createTableCommand, connection)) {
            connection.Open();
            command.ExecuteNonQuery();
          }
        }
      }
    }

    /// <summary>
    /// Tests if a given database exists
    /// </summary>
    /// <param name="databaseName"></param>
    /// <returns></returns>
    public bool DatabaseExists(string databaseName) {
      using (var connection = new SqlConnection(getMasterDatabaseConnectionString())) {
        string queryString = "SELECT Count(*) FROM sys.databases WHERE [name] = '" + databaseName + "'";
        using (var command = new SqlCommand(queryString, connection)) {
          connection.Open();
          int result = (int)command.ExecuteScalar();
          return result > 0;
        }
      }
    }

    /// <summary>
    /// Gets all Databases
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IDatabase> GetAllDatabases() {
      var databases = new List<IDatabase>();
      string queryString = "SELECT ID, Name, Description FROM Databases";

      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {       
        using (var command = new SqlCommand(queryString, connection)) {
          connection.Open();

          SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
          while (dataReader.Read()) {
            string databaseName = dataReader["Name"].ToString();
            databases.Add(new SqlServerDatabase(this, databaseName) {
              Id = Convert.ToInt32(dataReader["ID"]),
              Description = dataReader["Description"].ToString()
            });
          }
        }      
      }
      return databases;
    }

    /// <summary>
    /// Gets the specified database
    /// </summary>
    /// <param name="databaseName"></param>
    /// <returns></returns>
    public IDatabase GetDatabase(string databaseName) {
      string queryString = "SELECT ID, Name, Description FROM Databases WHERE Name=@Name";

      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(queryString, connection)) {
          command.Parameters.AddWithValue("@Name", databaseName);
          connection.Open();
          SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
          dataReader.Read();
          return new SqlServerDatabase(this, databaseName) {
            Id = Convert.ToInt32(dataReader["ID"]),
            Description = dataReader["Description"].ToString()
          };
        }
      }
    }

    /// <summary>
    /// Creates a new database
    /// </summary>
    /// <param name="database">The database to create</param>
    /// <returns></returns>
    public IDatabase CreateDatabase(IDatabase database) {
      string insertCommand = "INSERT INTO Databases (Name, Description) Values (@Name, @Description); SELECT SCOPE_IDENTITY()";
      createDatabase(database.Name);
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(insertCommand, connection)) {
          command.Parameters.AddWithValue("@Name", database.Name);
          command.Parameters.AddWithValue("@Description", database.Description ?? string.Empty);
          connection.Open();
          int id = Convert.ToInt32(command.ExecuteScalar());
          return new SqlServerDatabase(this, database.Name) {
            Id = id,
            Description = database.Description
          };
        }
      }
    }

    /// <summary>
    /// Updates the Description of the database.
    /// </summary>
    /// <param name="database">The database to update</param>
    /// <returns>Updated database</returns>
    public IDatabase UpdateDatabase(IDatabase database) {
      string updateCommand = "UPDATE Databases SET Description=@Description WHERE Name=@Name";
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(updateCommand, connection)) {
          command.Parameters.AddWithValue("@Description", database.Description);
          command.Parameters.AddWithValue("@Name", database.Name);
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
      return database;
    }

    /// <summary>
    /// Deletes the database from Taikun and drops the Database
    /// </summary>
    /// <param name="database">database to delete</param>
    public void DeleteDatabase(IDatabase database) {
      string deleteDatabaseCommand = string.Format("DELETE FROM [Databases] WHERE [Name]='{0}'", database.Name);
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(deleteDatabaseCommand, connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
      dropDatabase(database.Name);
    }

    private void createDatabase(string databaseName) {
      string createDatabaseCommand = string.Format("CREATE DATABASE [{0}]", databaseName);
      using (var connection = new SqlConnection(getMasterDatabaseConnectionString())) {
        using (var command = new SqlCommand(createDatabaseCommand, connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
    }

    private void dropDatabase(string databaseName) {
      string dropDatabaseCommand = string.Format("DROP DATABASE [{0}]", databaseName);
      using (var connection = new SqlConnection(getMasterDatabaseConnectionString())) {
        using (var command = new SqlCommand(dropDatabaseCommand, connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
    }

    private string getMasterDatabaseConnectionString() {
      var builder = new SqlConnectionStringBuilder(connectionStringBuilder.ConnectionString);
      builder.InitialCatalog = "master";
      return builder.ConnectionString;
    }

    public string GetDatabaseConnectionString(string databaseName) {
      var builder = new SqlConnectionStringBuilder(connectionStringBuilder.ConnectionString);
      builder.InitialCatalog = databaseName;
      return builder.ConnectionString;
    }
  }
}
