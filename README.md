Taikun
=====

A simple database management system

**Example Usage:**
```csharp
string connectionString = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=Taikun;Integrated Security=True";

// Create the Taikun database to manage all other databases we create
var projectManager = new SqlServerProjectManager(connectionString, createIfNotExists: true);

// Create a new database
IProject project = projectManager.CreateProject(new SqlServerProject {
  DatabaseName = "Test",
  Description = "This is a test"
});

// Create a new table in the newly created database  
DataTable dataTable = new DataTable("NewTable");
dataTable.Columns.Add("ID", typeof(int));
dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ID"] };
dataTable.Columns.Add("Something", typeof(string));
dataTable.Columns.Add(new DataColumn("VeryLongText", typeof(string)){ MaxLength = int.MaxValue });    
IProjectTable table = new SqlServerProjectTable(dataTable);
projectManager.CreateProjectTable(project, table);
```