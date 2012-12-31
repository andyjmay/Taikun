using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Taikun.SqlServer.Helpers;

namespace Taikun.SqlServer {
  public class SqlServerDatabase : IDatabase<SqlServerDatabaseTable> {
    public int Id { get; set; }
    public string Name { get; private set; }
    public string Description { get; set; }
    public string ConnectionString { get; private set; }

    public SqlServerDatabase(IDatabaseManager<SqlServerDatabase> databaseManager, string name) {
      this.Name = name;
      this.ConnectionString = databaseManager.GetDatabaseConnectionString(name);
    }

    public bool DatabaseTableExists(string tableName) {
      string queryString = string.Format("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}'", tableName);
      using (var connection = new SqlConnection(ConnectionString)) {
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
    public SqlServerDatabaseTable CreateDatabaseTable(SqlServerDatabaseTable databaseTable) {
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
      
      using (var connection = new SqlConnection(ConnectionString)) {
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
      return sqlServerDatabaseTable;
    }

    public void AddConstraint(SqlServerDatabaseTable sqlServerDatabaseTable) {
      var createRelationshipBuilder = new StringBuilder();
      if (sqlServerDatabaseTable.Schema.Constraints.Count > 0) {
        foreach (var constraint in sqlServerDatabaseTable.Schema.Constraints) {
          if (constraint is ForeignKeyConstraint) {
            var foreignKeyConstraint = constraint as ForeignKeyConstraint;
            createRelationshipBuilder.Append(string.Format(
              " ALTER TABLE [{0}] ADD CONSTRAINT {1} FOREIGN KEY ([{2}]) REFERENCES [{3}] ([{4}]) ",
                foreignKeyConstraint.Columns[0].Table, foreignKeyConstraint.ConstraintName, foreignKeyConstraint.Columns[0].ColumnName,
                foreignKeyConstraint.RelatedColumns[0].Table, foreignKeyConstraint.RelatedColumns[0].ColumnName));
          }
        }
      }
      if (createRelationshipBuilder.Length > 0) {
        using (var connection = new SqlConnection(ConnectionString)) {
          using (var command = new SqlCommand(createRelationshipBuilder.ToString(), connection)) {
            if (connection.State != ConnectionState.Open) {
              connection.Open();
            }
            command.ExecuteNonQuery();
          }
        }
      }
    }

    public SqlServerDatabaseTable GetDatabaseTable(string tableName, bool loadData) {
      string selectQuery;
      if (loadData) {
        selectQuery = string.Format("SELECT * FROM [{0}]", tableName);
      } else {
        selectQuery = string.Format("SELECT TOP 0 * FROM [{0}]", tableName);
      }
      using (var connection = new SqlConnection(ConnectionString)) {
        using (var dataAdapter = new SqlDataAdapter(selectQuery, connection)) {
          connection.Open();
          var dataTable = new DataTable();
          dataAdapter.FillSchema(dataTable, SchemaType.Source);
          dataAdapter.Fill(dataTable);
          return new SqlServerDatabaseTable(dataTable);
        }
      }
    }

    public IEnumerable<SqlServerDatabaseTable> GetDatabaseTables() {
      string selectQuery = "SELECT * FROM sys.Tables";
      var databaseTables = new List<SqlServerDatabaseTable>();
      using (var connection = new SqlConnection(ConnectionString)) {
        using (var command = new SqlCommand(selectQuery, connection)) {
          connection.Open();
          SqlDataReader dataReader = command.ExecuteReader();
          while (dataReader.Read()) {
            databaseTables.Add(GetDatabaseTable(dataReader["name"].ToString(), false));
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
    public void DeleteDatabaseTable(SqlServerDatabaseTable databaseTable) {
      string deleteCommand = string.Format("DROP TABLE [{0}]", databaseTable.Name);
      using (var connection = new SqlConnection(ConnectionString)) {
        using (var command = new SqlCommand(deleteCommand, connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
    }
  }
}
