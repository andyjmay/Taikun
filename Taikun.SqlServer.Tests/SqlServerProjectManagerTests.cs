using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace Taikun.SqlServer.Tests {
  [TestClass]
  public class SqlServerProjectManagerTests {
    private const string dataSource = @"(local)\SQLEXPRESS";
    private const string database = "Taikun";
    private readonly SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", dataSource, database));

    [TestMethod]
    public void GetAllProjects_GetsAllProjects() {
      var projectManager = new SqlServerProjectManager(connectionStringBuilder.ConnectionString, true);
      var createdProjects = new List<IProject>();
      for (int i = 0; i < 10; i++) {
        createdProjects.Add(projectManager.CreateProject(getRandomProject()));
      }
      IEnumerable<IProject> projects = projectManager.GetAllProjects();
      Assert.IsTrue(projects.Any());
      deleteAllProjects();
    }
    
    [TestMethod]
    public void GetProject_GetsProject() {
      var projectManager = new SqlServerProjectManager(connectionStringBuilder.ConnectionString, true);
      var createdProjects = new List<IProject>();
      for (int i = 0; i < 10; i++) {
        createdProjects.Add(projectManager.CreateProject(getRandomProject()));
      }
      IProject projectToFind = createdProjects[new Random().Next(0, createdProjects.Count - 1)];
      IProject projectFound = projectManager.GetProject(projectToFind.DatabaseName);
      Assert.AreEqual(projectToFind.DatabaseName, projectFound.DatabaseName);
      Assert.AreEqual(projectToFind.Description, projectFound.Description);
      deleteAllProjects();
    }

    [TestMethod]
    public void CreateProject_CreatesProject() {
      var projectManager = new SqlServerProjectManager(connectionStringBuilder.ConnectionString, true);
      IProject createdProject = projectManager.CreateProject(getRandomProject());
      Assert.IsNotNull(createdProject);
      Assert.IsTrue(databaseExists(createdProject.DatabaseName));
      deleteAllProjects();
    }

    [TestMethod]
    public void UpdateProject_UpdatesProject() {
      var projectManager = new SqlServerProjectManager(connectionStringBuilder.ConnectionString, true);
      IProject createdProject = projectManager.CreateProject(getRandomProject());
      string newDescription = DateTime.Now.ToFileTimeUtc().ToString();
      createdProject.Description = newDescription;
      projectManager.UpdateProject(createdProject);

      IProject updatedProject = projectManager.GetProject(createdProject.DatabaseName);
      projectManager.DeleteProject(createdProject);
      Assert.AreEqual(createdProject.Description, updatedProject.Description);
      deleteAllProjects();
    }

    [TestMethod]
    public void DeleteProject_DeletesProject() {
      var projectManager = new SqlServerProjectManager(connectionStringBuilder.ConnectionString, true);
      IProject createdProject = projectManager.CreateProject(getRandomProject());
      projectManager.DeleteProject(createdProject);
      Assert.IsFalse(databaseExists(createdProject.DatabaseName));
    }

    private void deleteAllProjects() {
      var projectManager = new SqlServerProjectManager(connectionStringBuilder.ConnectionString, true);
      foreach (IProject project in projectManager.GetAllProjects()) {
        projectManager.DeleteProject(project);
      }
    }

    private IProject getRandomProject() {
      return new SqlServerProject {
        DatabaseName = Guid.NewGuid().ToString(),
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
