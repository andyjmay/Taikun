using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Taikun.SqlServer {
  public class SqlServerDatabaseManager : IDatabaseManager<SqlServerDatabase, SqlServerDatabaseTable> {
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
        const string createTableCommand = "CREATE TABLE [dbo].[Databases]([ID] [int] IDENTITY(1,1) NOT NULL,[Name] [nvarchar](255) NOT NULL,[Description] [nvarchar](255) NULL, CONSTRAINT [PK_Databases] PRIMARY KEY CLUSTERED([ID] ASC))";
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
    /// <param name="name"></param>
    /// <returns></returns>
    public bool DatabaseExists(string name) {
      using (var connection = new SqlConnection(getMasterDatabaseConnectionString())) {
        string queryString = "SELECT Count(*) FROM sys.databases WHERE [name] = '" + name + "'";
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
    public IEnumerable<SqlServerDatabase> GetAllDatabases() {
      var databases = new List<SqlServerDatabase>();
      const string queryString = "SELECT ID, Name, Description FROM Databases";

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
    /// <param name="name"></param>
    /// <returns></returns>
    public SqlServerDatabase GetDatabase(string name) {
      const string queryString = "SELECT ID, Name, Description FROM Databases WHERE Name=@Name";

      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(queryString, connection)) {
          command.Parameters.AddWithValue("@Name", name);
          connection.Open();
          SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
          dataReader.Read();
          return new SqlServerDatabase(this, name) {
            Id = Convert.ToInt32(dataReader["ID"]),
            Description = dataReader["Description"].ToString()
          };
        }
      }
    }

    /// <summary>
    /// Adds an existing database to Taikun
    /// </summary>
    /// <param name="name">The name of the database to add</param>
    /// <param name="description">The description for this database</param>
    /// <returns></returns>
    public SqlServerDatabase AddExistingDatabase(string name, string description) {
      int databaseId = addDatabaseToTaikun(name, description);
      return new SqlServerDatabase(this, name) {
        Id = databaseId,
        Description = description
      };
    }

    /// <summary>
    /// Creates a new database
    /// </summary>
    /// <param name="name">The name of the database to create</param>
    /// <param name="description">The description of the database to create</param>
    /// <returns></returns>
    public SqlServerDatabase CreateDatabase(string name, string description) {
      createDatabase(name);
      int databaseId = addDatabaseToTaikun(name, description);
      return new SqlServerDatabase(this, name) {
        Id = databaseId,
        Description = description
      };
    }

    /// <summary>
    /// Updates the Description of the database.
    /// </summary>
    /// <param name="name">The database to update</param>
    /// <param name="description">The updated description</param>
    /// <returns>Updated database</returns>
    public void UpdateDatabaseDescription(string name, string description) {
      const string updateCommand = "UPDATE Databases SET Description=@Description WHERE Name=@Name";
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(updateCommand, connection)) {
          command.Parameters.AddWithValue("@Description", description);
          command.Parameters.AddWithValue("@Name", name);
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
    }

    /// <summary>
    /// Deletes the database from Taikun and drops the Database
    /// </summary>
    /// <param name="name">The name of the database to delete</param>
    public void DeleteDatabase(string name) {
      string deleteDatabaseCommand = string.Format("DELETE FROM [Databases] WHERE [Name]='{0}'", name);
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(deleteDatabaseCommand, connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
      dropDatabase(name);
    }

    private int addDatabaseToTaikun(string name, string description) {
      const string insertCommand = "INSERT INTO Databases (Name, Description) Values (@Name, @Description); SELECT SCOPE_IDENTITY()";
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(insertCommand, connection)) {
          command.Parameters.AddWithValue("@Name", name);
          command.Parameters.AddWithValue("@Description", description ?? string.Empty);
          connection.Open();
          int id = Convert.ToInt32(command.ExecuteScalar());
          return id;
        }
      }
    }

    private void createDatabase(string name) {
      string createDatabaseCommand = string.Format("CREATE DATABASE [{0}]", name);
      using (var connection = new SqlConnection(getMasterDatabaseConnectionString())) {
        using (var command = new SqlCommand(createDatabaseCommand, connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
    }

    private void dropDatabase(string name) {
      string dropDatabaseCommand = string.Format("DROP DATABASE [{0}]", name);
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

    public string GetDatabaseConnectionString(string name) {
      var builder = new SqlConnectionStringBuilder(connectionStringBuilder.ConnectionString);
      builder.InitialCatalog = name;
      return builder.ConnectionString;
    }
  }
}
