Taikun
=====

A simple database management system

**Example Usage:**
```csharp
string connectionString = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=Taikun;Integrated Security=True";

// Create the Taikun database to manage all other databases we create
var databaseManager = new SqlServerDatabaseManager(connectionString, createIfNotExists: true);

// Create a new database
var database = databaseManager.CreateDatabase(name: "Test", description: "This is a test");

// Create a new table in the newly created database  
DataTable dataTable = new DataTable("NewTable");
dataTable.Columns.Add("ID", typeof(int));
dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["ID"] };
dataTable.Columns.Add("Something", typeof(string));
dataTable.Columns.Add(new DataColumn("VeryLongText", typeof(string)){ MaxLength = int.MaxValue });    
database.CreateDatabaseTable(new SqlServerDatabaseTable(databaseManager, dataTable));
```