using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Taikun.SqlServer {
  public class SqlServerProjectManager : IProjectManager {
    private string connectionString;

    public SqlServerProjectManager(string connectionString) {
      this.connectionString = connectionString;
    }

    public IQueryable<IProject> GetAllProjects() {
      var projects = new List<IProject>();
      string queryString = "SELECT ID, DatabaseName, Description FROM Projects";

      using (SqlConnection connection = new SqlConnection(connectionString)) {       
        using (SqlCommand command = new SqlCommand(queryString, connection)) {
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
      return projects.AsQueryable();
    }

    public IProject CreateProject(IProject project) {
      string insertCommand = "INSERT INTO Projects (DatabaseName, Description) Values (@DatabaseName, @Description); SELECT SCOPE_IDENTITY()";
      using (SqlConnection connection = new SqlConnection(connectionString)) {
        using (SqlCommand command = new SqlCommand(insertCommand, connection)) {
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

    public IProject UpdateProject(IProject project) {
      throw new NotImplementedException();
    }

    public void DeleteProject(IProject project) {
      throw new NotImplementedException();
    }
  }
}
