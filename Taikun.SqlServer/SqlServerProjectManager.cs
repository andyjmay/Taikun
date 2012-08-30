using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Taikun.SqlServer.Helpers;

namespace Taikun.SqlServer {
  public class SqlServerProjectManager : IProjectManager {
    private SqlConnectionStringBuilder connectionStringBuilder;

    public SqlServerProjectManager(string connectionString, bool createIfNotExists = false) {
      connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      if (createIfNotExists && !databaseExists(connectionStringBuilder.InitialCatalog)) {
        createDatabase(connectionStringBuilder.InitialCatalog);
        string createTableCommand = "CREATE TABLE [dbo].[Projects]([ID] [int] IDENTITY(1,1) NOT NULL,[DatabaseName] [varchar](255) NOT NULL,[Description] [varchar](255) NULL)";
        using (var connection = new SqlConnection(getDatabaseConnectionString(connectionStringBuilder.InitialCatalog))) {
          using (var command = new SqlCommand(createTableCommand, connection)) {
            connection.Open();
            command.ExecuteNonQuery();
          }
        }
      }
    }

    /// <summary>
    /// Gets all Projects
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IProject> GetAllProjects() {
      var projects = new List<IProject>();
      string queryString = "SELECT ID, DatabaseName, Description FROM Projects";

      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {       
        using (var command = new SqlCommand(queryString, connection)) {
          connection.Open();

          SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
          while (dataReader.Read()) {
            projects.Add(new SqlServerProject {
              Id = Convert.ToInt32(dataReader["ID"]),
              DatabaseName = dataReader["DatabaseName"].ToString(),
              Description = dataReader["Description"].ToString()
            });
          }
        }      
      }
      return projects;
    }

    /// <summary>
    /// Gets the specified project
    /// </summary>
    /// <param name="databaseName"></param>
    /// <returns></returns>
    public IProject GetProject(string databaseName) {
      string queryString = "SELECT ID, DatabaseName, Description FROM Projects WHERE DatabaseName=@DatabaseName";

      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(queryString, connection)) {
          command.Parameters.AddWithValue("@DatabaseName", databaseName);
          connection.Open();
          SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
          dataReader.Read();
          return new SqlServerProject {
            Id = Convert.ToInt32(dataReader["ID"]),
            DatabaseName = dataReader["DatabaseName"].ToString(),
            Description = dataReader["Description"].ToString()
          };
        }
      }
    }

    /// <summary>
    /// Creates a new Project
    /// </summary>
    /// <param name="project">The Project to create</param>
    /// <returns></returns>
    public IProject CreateProject(IProject project) {
      string insertCommand = "INSERT INTO Projects (DatabaseName, Description) Values (@DatabaseName, @Description); SELECT SCOPE_IDENTITY()";
      createDatabase(project.DatabaseName);
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(insertCommand, connection)) {
          command.Parameters.AddWithValue("@DatabaseName", project.DatabaseName);
          command.Parameters.AddWithValue("@Description", project.Description);
          connection.Open();
          int id = Convert.ToInt32(command.ExecuteScalar());
          return new SqlServerProject {
            Id = id,
            DatabaseName = project.DatabaseName,
            Description = project.Description
          };
        }
      }
    }

    /// <summary>
    /// Updates the Description of the Project.
    /// </summary>
    /// <param name="project">The Project to update</param>
    /// <returns>Updated Project</returns>
    public IProject UpdateProject(IProject project) {
      string updateCommand = "UPDATE Projects SET Description=@Description WHERE DatabaseName=@DatabaseName";
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(updateCommand, connection)) {
          command.Parameters.AddWithValue("@Description", project.Description);
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
      return project;
    }

    /// <summary>
    /// Deletes the Project from Taikun and drops the Database
    /// </summary>
    /// <param name="project">Project to delete</param>
    public void DeleteProject(IProject project) {
      string deleteProjectCommand = string.Format("DELETE FROM [Projects] WHERE [DatabaseName]='{0}'", project.DatabaseName);
      using (var connection = new SqlConnection(connectionStringBuilder.ConnectionString)) {
        using (var command = new SqlCommand(deleteProjectCommand, connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
      dropDatabase(project.DatabaseName);
    }

    /// <summary>
    /// Creates a new table in the specified database
    /// </summary>
    /// <param name="project"></param>
    /// <param name="projectTable"></param>
    public void CreateProjectTable(IProject project, IProjectTable projectTable) {
      if (!(projectTable is SqlServerProjectTable)) {
        throw new ArgumentException("The project table must be a SQL project table");
      }
      var sqlServerProjectTable = projectTable as SqlServerProjectTable;
      var createTableBuilder = new StringBuilder(string.Format("CREATE TABLE [{0}] (", sqlServerProjectTable.Name));
      foreach (DataColumn column in sqlServerProjectTable.Schema.Columns) {
        createTableBuilder.Append(string.Format("[{0}] {1}", column.ColumnName, column.GetSqlType()));
        createTableBuilder.Append(",");
      }
      createTableBuilder = createTableBuilder.Remove(createTableBuilder.Length - 1, 1);
      if (sqlServerProjectTable.Schema.PrimaryKey.Length > 0) {
        createTableBuilder.Append(" CONSTRAINT [PK_" + sqlServerProjectTable.Schema.TableName + "] PRIMARY KEY CLUSTERED (");
        foreach (DataColumn column in sqlServerProjectTable.Schema.PrimaryKey) {
          createTableBuilder.Append("[" + column.ColumnName + "],");
        }
        createTableBuilder = createTableBuilder.Remove(createTableBuilder.Length - 1, 1);
        createTableBuilder.Append(")");
      }
      createTableBuilder.Append(")");
      using (var connection = new SqlConnection(getDatabaseConnectionString(project.DatabaseName))) {
        using (var command = new SqlCommand(createTableBuilder.ToString(), connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
    }

    /// <summary>
    /// Deletes a table from the specified database
    /// </summary>
    /// <param name="project"></param>
    /// <param name="projectTable"></param>
    public void DeleteProjectTable(IProject project, IProjectTable projectTable) {
      string deleteCommand = string.Format("DROP TABLE [{0}]", projectTable.Name);
      using (var connection = new SqlConnection(getDatabaseConnectionString(project.DatabaseName))) {
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

    private bool databaseExists(string databaseName) {
      using (var connection = new SqlConnection(getMasterDatabaseConnectionString())) {
        string queryString = "SELECT Count(*) FROM sys.databases WHERE [name] = '" + databaseName + "'";
        using (var command = new SqlCommand(queryString, connection)) {
          connection.Open();
          try {
            int result = (int)command.ExecuteScalar();
            return result > 0;
          }
          catch (SqlException ex) {
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

    private string getDatabaseConnectionString(string databaseName) {
      var builder = new SqlConnectionStringBuilder(connectionStringBuilder.ConnectionString);
      builder.InitialCatalog = databaseName;
      return builder.ConnectionString;
    }

  }
}
