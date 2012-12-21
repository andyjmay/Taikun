using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Taikun.SqlServer.Helpers;

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
        string createTableCommand = "CREATE TABLE [dbo].[Databases]([ID] [int] IDENTITY(1,1) NOT NULL,[DatabaseName] [nvarchar](255) NOT NULL,[Description] [nvarchar](255) NULL)";
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
      string queryString = "SELECT ID, DatabaseName, Description FROM Databases";

      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {       
        using (var command = new SqlCommand(queryString, connection)) {
          connection.Open();

          SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
          while (dataReader.Read()) {
            databases.Add(new SqlServerDatabase {
              Id = Convert.ToInt32(dataReader["ID"]),
              DatabaseName = dataReader["DatabaseName"].ToString(),
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
      string queryString = "SELECT ID, DatabaseName, Description FROM Databases WHERE DatabaseName=@DatabaseName";

      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(queryString, connection)) {
          command.Parameters.AddWithValue("@DatabaseName", databaseName);
          connection.Open();
          SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
          dataReader.Read();
          return new SqlServerDatabase {
            Id = Convert.ToInt32(dataReader["ID"]),
            DatabaseName = dataReader["DatabaseName"].ToString(),
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
      string insertCommand = "INSERT INTO Databases (DatabaseName, Description) Values (@DatabaseName, @Description); SELECT SCOPE_IDENTITY()";
      createDatabase(database.DatabaseName);
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(insertCommand, connection)) {
          command.Parameters.AddWithValue("@DatabaseName", database.DatabaseName);
          command.Parameters.AddWithValue("@Description", database.Description);
          connection.Open();
          int id = Convert.ToInt32(command.ExecuteScalar());
          return new SqlServerDatabase {
            Id = id,
            DatabaseName = database.DatabaseName,
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
      string updateCommand = "UPDATE Databases SET Description=@Description WHERE DatabaseName=@DatabaseName";
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(updateCommand, connection)) {
          command.Parameters.AddWithValue("@Description", database.Description);
          command.Parameters.AddWithValue("@DatabaseName", database.DatabaseName);
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
      string deleteDatabaseCommand = string.Format("DELETE FROM [Databases] WHERE [DatabaseName]='{0}'", database.DatabaseName);
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(deleteDatabaseCommand, connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
      dropDatabase(database.DatabaseName);
    }

    public bool DatabaseTableExists(IDatabase database, string tableName) {
      string queryString = string.Format("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}'", tableName);
      using (var connection = new SqlConnection(GetDatabaseConnectionString(database.DatabaseName))) {
        using (var command = new SqlCommand(queryString, connection)) {
          connection.Open();
          int result = (int)command.ExecuteScalar();
          return result > 0;
        }
      }
    }

    /// <summary>
    /// Creates a new table in the specified database
    /// </summary>
    /// <param name="database"></param>
    /// <param name="databaseTable"></param>
    public void CreateDatabaseTable(IDatabase database, IDatabaseTable databaseTable) {
      if (!(databaseTable is SqlServerDatabaseTable)) {
        throw new ArgumentException("The database table must be a SQL database table");
      }
      var sqlServerDatabaseTable = databaseTable as SqlServerDatabaseTable;
      var createTableBuilder = new StringBuilder(string.Format("CREATE TABLE [{0}] (", sqlServerDatabaseTable.Name));
      foreach (DataColumn column in sqlServerDatabaseTable.Schema.Columns) {
        createTableBuilder.Append(string.Format("[{0}] {1}", column.ColumnName, column.GetSqlType()));
        if (column.AutoIncrement) {
          createTableBuilder.Append(string.Format(" IDENTITY ({0},{1})", column.AutoIncrementSeed, column.AutoIncrementStep));
        }
        createTableBuilder.Append(column.AllowDBNull ? " NULL" : " NOT NULL");
        createTableBuilder.Append(",");
      }
      createTableBuilder = createTableBuilder.Remove(createTableBuilder.Length - 1, 1);
      if (sqlServerDatabaseTable.Schema.PrimaryKey.Length > 0) {
        createTableBuilder.Append(" CONSTRAINT [PK_" + sqlServerDatabaseTable.Schema.TableName + "] PRIMARY KEY CLUSTERED (");
        foreach (DataColumn column in sqlServerDatabaseTable.Schema.PrimaryKey) {
          createTableBuilder.Append("[" + column.ColumnName + "],");
        }
        createTableBuilder = createTableBuilder.Remove(createTableBuilder.Length - 1, 1);
        createTableBuilder.Append(")");
      }
      createTableBuilder.Append(")");
      using (var connection = new SqlConnection(GetDatabaseConnectionString(database.DatabaseName))) {
        using (var command = new SqlCommand(createTableBuilder.ToString(), connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
        if (sqlServerDatabaseTable.Schema.Rows.Count > 0) {
          using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers, null)) {
            bulkCopy.DestinationTableName = "[" + sqlServerDatabaseTable.Name + "]";
            if (connection.State != ConnectionState.Open) {
              connection.Open();
            }
            bulkCopy.WriteToServer(sqlServerDatabaseTable.Schema);
          }
        }
      }
    }
    
    public IDatabaseTable GetDatabaseTable(IDatabase database, string tableName, bool loadData) {
      string selectQuery;
      if (loadData) {
        selectQuery = string.Format("SELECT * FROM [{0}]", tableName);
      } else {
        selectQuery = string.Format("SELECT TOP 0 * FROM [{0}]", tableName);
      }
      using (var connection = new SqlConnection(GetDatabaseConnectionString(database.DatabaseName))) {
        using (var dataAdapter = new SqlDataAdapter(selectQuery, connection)) {
          connection.Open();
          var dataTable = new DataTable();
          dataAdapter.FillSchema(dataTable, SchemaType.Source);
          dataAdapter.Fill(dataTable);
          return new SqlServerDatabaseTable(dataTable);
        }
      }
    }

    public IEnumerable<IDatabaseTable> GetDatabaseTables(IDatabase database) {
      string selectQuery = "SELECT * FROM sys.Tables";
      var databaseTables = new List<IDatabaseTable>();
      using (var connection = new SqlConnection(GetDatabaseConnectionString(database.DatabaseName))) {
        using (var command = new SqlCommand(selectQuery, connection)) {
          connection.Open();
          SqlDataReader dataReader = command.ExecuteReader();
          while (dataReader.Read()) {
            databaseTables.Add(GetDatabaseTable(database, dataReader["name"].ToString(), false));
          }
        }
      }
      return databaseTables;
    }

    /// <summary>
    /// Deletes a table from the specified database
    /// </summary>
    /// <param name="database"></param>
    /// <param name="databaseTable"></param>
    public void DeleteDatabaseTable(IDatabase database, IDatabaseTable databaseTable) {
      string deleteCommand = string.Format("DROP TABLE [{0}]", databaseTable.Name);
      using (var connection = new SqlConnection(GetDatabaseConnectionString(database.DatabaseName))) {
        using (var command = new SqlCommand(deleteCommand, connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
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
