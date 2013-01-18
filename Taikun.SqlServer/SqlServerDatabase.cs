using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using Taikun.SqlServer.Helpers;

namespace Taikun.SqlServer {
  public class SqlServerDatabase : IDatabase<SqlServerDatabaseTable> {
    public int Id { get; set; }
    public string Name { get; private set; }
    public string Description { get; set; }
    public string ConnectionString { get; private set; }

    internal SqlServerDatabase(IDatabaseManager<SqlServerDatabase> databaseManager, string name) {
      this.Name = name;
      this.ConnectionString = databaseManager.GetDatabaseConnectionString(name);
    }

    public bool DatabaseTableExists(string tableName) {
      tableName = tableName.Replace("[", "").Replace("]", "");
      string queryString = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE ";

      if (tableName.Contains(".")) {
        string[] tableNameArray = tableName.Split('.');
        if (tableNameArray.Length != 2) {
          throw new ArgumentException("The table name can either be the name of the table, or the schema and table name: dbo.MyTableName", tableName);
        }
        queryString += string.Format("TABLE_NAME = '{0}' AND TABLE_SCHEMA = '{1}'", tableNameArray[1], tableNameArray[0]);
      }
      else {
        queryString += string.Format("TABLE_NAME = '{0}'", tableName);
      }
      
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
      if (databaseTable.Schema.Columns.Count == 0) {
        throw new ArgumentException("The table must contain columns in order to create it.");
      }
      if (string.IsNullOrWhiteSpace(databaseTable.Schema.Namespace)) {
        databaseTable.Schema.Namespace = "dbo";
      }
      var createTableBuilder = new StringBuilder(string.Format("CREATE TABLE [{0}].[{1}] (", databaseTable.Schema.Namespace, databaseTable.Name));

      foreach (DataColumn column in databaseTable.Schema.Columns) {
        createTableBuilder.Append(string.Format("[{0}] {1}", column.ColumnName, column.GetSqlType()));
        if (column.AutoIncrement) {
          createTableBuilder.Append(string.Format(" IDENTITY ({0},{1})", column.AutoIncrementSeed, column.AutoIncrementStep));
        }
        createTableBuilder.Append(column.AllowDBNull ? " NULL" : " NOT NULL");
        createTableBuilder.Append(",");
      }
      createTableBuilder = createTableBuilder.Remove(createTableBuilder.Length - 1, 1);

      if (databaseTable.Schema.PrimaryKey.Length > 0) {
        createTableBuilder.Append(", CONSTRAINT [PK_" + databaseTable.Schema.TableName + "] PRIMARY KEY CLUSTERED (");
        foreach (DataColumn column in databaseTable.Schema.PrimaryKey) {
          createTableBuilder.Append("[" + column.ColumnName + "] ASC,");
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
        using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers, null)) {
          bulkCopy.DestinationTableName = string.Format("[{0}].[{1}]", databaseTable.Schema.Namespace, databaseTable.Name);
          if (connection.State != ConnectionState.Open) {
            connection.Open();
          }
          bulkCopy.WriteToServer(databaseTable.Schema);
        }
      }
      return databaseTable;
    }

    public void AddForeignKeyConstraint(SqlServerDatabaseTable table, ForeignKeyConstraint foreignKeyConstraint) {
      var createRelationshipBuilder = new StringBuilder();
      if (string.IsNullOrWhiteSpace(table.Schema.Namespace)) {
        table.Schema.Namespace = "dbo";
      }
      if (string.IsNullOrWhiteSpace(foreignKeyConstraint.ConstraintName)) {
        foreignKeyConstraint.ConstraintName = "FK_" + foreignKeyConstraint.Columns[0].ColumnName + "_" + foreignKeyConstraint.RelatedColumns[0].ColumnName;
      }
      createRelationshipBuilder.Append(string.Format(
        " ALTER TABLE [{0}].[{1}] ADD CONSTRAINT {2} FOREIGN KEY ([{3}]) REFERENCES [{4}] ([{5}]) ",
          table.Schema.Namespace, table.Schema.TableName, foreignKeyConstraint.ConstraintName, foreignKeyConstraint.Columns[0].ColumnName,
          foreignKeyConstraint.RelatedColumns[0].Table, foreignKeyConstraint.RelatedColumns[0].ColumnName));

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

    public SqlServerDatabaseTable GetDatabaseTable(string tableName) {
      return GetDatabaseTable("dbo", tableName);
    }

    public SqlServerDatabaseTable GetDatabaseTable(string schema, string tableName) {
      string selectQuery = string.Format("SELECT TOP 0 * FROM [{0}]", tableName);

      using (var connection = new SqlConnection(ConnectionString)) {
        using (var dataAdapter = new SqlDataAdapter(selectQuery, connection)) {
          connection.Open();
          var dataTable = new DataTable(tableName);
          dataAdapter.FillSchema(dataTable, SchemaType.Source);
          dataAdapter.Fill(dataTable);
          return new SqlServerDatabaseTable(dataTable);
        }
      }
    }

    public IEnumerable<SqlServerDatabaseTable> GetDatabaseTables() {
      string selectQuery = "SELECT schemas.name AS Namespace, Tables.name AS TableName FROM sys.Tables INNER JOIN sys.schemas ON sys.Tables.schema_id = sys.schemas.schema_id";
      using (var connection = new SqlConnection(ConnectionString)) {
        using (var command = new SqlCommand(selectQuery, connection)) {
          connection.Open();
          SqlDataReader dataReader = command.ExecuteReader();
          while (dataReader.Read()) {
            yield return GetDatabaseTable(dataReader["Namespace"].ToString(), dataReader["TableName"].ToString());
          }
        }
      }
    }

    /// <summary>
    /// Deletes a table from the specified database
    /// </summary>
    /// <param name="tableName"></param>
    public void DeleteDatabaseTable(string tableName) {
      string deleteCommand = string.Format("DROP TABLE {0}", tableName);
      using (var connection = new SqlConnection(ConnectionString)) {
        using (var command = new SqlCommand(deleteCommand, connection)) {
          connection.Open();
          command.ExecuteNonQuery();
        }
      }
    }

    // Inspired by Massive https://github.com/robconery/massive
    public IEnumerable<dynamic> Query(string sql, params object[] args) {
      using (var connection = new SqlConnection(ConnectionString)) {
        connection.Open();
        var reader = createCommand(sql, connection, args).ExecuteReader();
        while (reader.Read()) {
          yield return reader.RecordToExpando();
        }
      }
    }

    public virtual object Scalar(string sql, params object[] args) {
      object result;
      using (var connection = new SqlConnection(ConnectionString)) {
        connection.Open();
        result = createCommand(sql, connection, args).ExecuteScalar();
      }
      return result;
    }

    public virtual IEnumerable<dynamic> All(string tableName, string where = "", string orderBy = "", int limit = 0, string columns = "*", params object[] args) {
      string sql = buildSelect(where, orderBy, limit);
      return Query(string.Format(sql, columns, tableName), args);
    }

    public virtual dynamic Single(string tableName, string where, params object[] args) {
      var sql = string.Format("SELECT * FROM {0} WHERE {1}", tableName, where);
      return Query(sql, args).FirstOrDefault();
    }

    public DataTable GetDataTable(string tableName, bool loadData = false) {
      string selectQuery;
      
      if (loadData) {
        selectQuery = string.Format("SELECT * FROM {0}", tableName);
      } else {
        selectQuery = string.Format("SELECT TOP 0 * FROM {0}", tableName);
      }

      using (var connection = new SqlConnection(ConnectionString)) {
        using (var dataAdapter = new SqlDataAdapter(selectQuery, connection)) {
          connection.Open();
          var dataTable = new DataTable();
          dataAdapter.FillSchema(dataTable, SchemaType.Source);
          dataAdapter.Fill(dataTable);
          return dataTable;
        }
      }
    }

    private DbCommand createCommand(string sql, DbConnection connection, params object[] args) {
      var command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
      command.Connection = connection;
      command.CommandText = sql;
      if (args.Length > 0)
        command.AddParams(args);
      return command;
    }

    private static string buildSelect(string where, string orderBy, int limit) {
      string sql = limit > 0 ? "SELECT TOP " + limit + " {0} FROM {1} " : "SELECT {0} FROM {1} ";
      if (!string.IsNullOrEmpty(where))
        sql += where.Trim().StartsWith("where", StringComparison.OrdinalIgnoreCase) ? where : " WHERE " + where;
      if (!String.IsNullOrEmpty(orderBy))
        sql += orderBy.Trim().StartsWith("order by", StringComparison.OrdinalIgnoreCase) ? orderBy : " ORDER BY " + orderBy;
      return sql;
    }
  }

  internal static class ObjectExtensions {
    /// <summary>
    /// Extension method for adding in a bunch of parameters
    /// </summary>
    public static void AddParams(this DbCommand cmd, params object[] args) {
      foreach (var item in args) {
        AddParam(cmd, item);
      }
    }

    /// <summary>
    /// Extension for adding single parameter
    /// </summary>
    public static void AddParam(this DbCommand cmd, object item) {
      var p = cmd.CreateParameter();
      p.ParameterName = string.Format("@{0}", cmd.Parameters.Count);
      if (item == null) {
        p.Value = DBNull.Value;
      } else {
        if (item is Guid) {
          p.Value = item.ToString();
          p.DbType = DbType.String;
          p.Size = 4000;
        } else if (item is ExpandoObject) {
          var d = (IDictionary<string, object>)item;
          p.Value = d.Values.FirstOrDefault();
        } else {
          p.Value = item;
        }
        if (item is string)
          p.Size = ((string)item).Length > 4000 ? -1 : 4000;
      }
      cmd.Parameters.Add(p);
    }

    public static dynamic RecordToExpando(this IDataReader rdr) {
      dynamic e = new ExpandoObject();
      var d = e as IDictionary<string, object>;
      for (int i = 0; i < rdr.FieldCount; i++)
        d.Add(rdr.GetName(i), DBNull.Value.Equals(rdr[i]) ? null : rdr[i]);
      return e;
    }
  }
}
