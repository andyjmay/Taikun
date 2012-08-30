Taikun
=====

A simple database management system

**Example Usage:**

    string connectionString = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=Taikun;Integrated Security=True";
    var projectManager = new SqlServerProjectManager(connectionString, createIfNotExists: true);
    projectManager.CreateProject(new SqlServerProject {
      DatabaseName = "Test",
      Description = "This is a test"
    });
    IEnumerable<IProject> projects = projectManager.GetAllProjects();