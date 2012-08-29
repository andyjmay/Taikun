using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Taikun.SqlServer.Tests {
  [TestClass]
  public class SqlServerProjectManagerTests {
    private string connectionString = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=Taikun;Integrated Security=True";
    [TestMethod]
    public void GetAllProjects_GetsAllProjects() {
      var projectManager = new SqlServerProjectManager(connectionString);
      IQueryable<IProject> projects = projectManager.GetAllProjects();
      Assert.IsTrue(projects.Count() > 0);     
    }
    
    [TestMethod]
    public void CreateProject_CreatesProject() {
      var projectManager = new SqlServerProjectManager(connectionString);
      IProject createdProject = projectManager.CreateProject(new SqlServerProject {
        DatabaseName = Guid.NewGuid().ToString(),
        Description = DateTime.Now.ToFileTimeUtc().ToString()
      });
      Assert.IsNotNull(createdProject);      
    }
  }
}
