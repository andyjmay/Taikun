using System;
using System.Data;

namespace Taikun.SqlServer.Helpers {
  public static class DataTypeExtensions {
    public static string GetSqlType(this DataColumn dataColumn) {
      switch (dataColumn.DataType.ToString()) {
        case "System.String":
          return "NVARCHAR(" + (dataColumn.MaxLength == -1 ? "255" : dataColumn.MaxLength > 8000 ? "MAX" : dataColumn.MaxLength.ToString()) + ")";

        case "System.Decimal":
          return "REAL";

        case "System.Double":
        case "System.Single":
          return "REAL";

        case "System.Int64":
          return "BIGINT";

        case "System.Int16":
        case "System.Int32":
          return "INT";

        case "System.DateTime":
          return "DATETIME";

        case "System.Boolean":
          return "BIT";

        case "System.Byte":
          return "TINYINT";

        case "System.Guid":
          return "UNIQUEIDENTIFIER";

        case "Microsoft.SqlServer.Types.SqlGeometry":
        case "System.Data.Spatial.DbGeometry":
          return "GEOMETRY";

        default:
          throw new Exception(dataColumn.DataType + " not implemented.");
      }
    }
    public static string GetSqlType(this Type type) {
      switch (type.ToString()) {
        case "System.String":
          return "NVARCHAR(MAX)";

        case "System.Decimal":
          return "REAL";

        case "System.Double":
        case "System.Single":
          return "REAL";

        case "System.Int64":
          return "BIGINT";

        case "System.Int16":
        case "System.Int32":
          return "INT";

        case "System.DateTime":
          return "DATETIME";

        case "System.Boolean":
          return "BIT";

        case "System.Byte":
          return "TINYINT";

        case "System.Guid":
          return "UNIQUEIDENTIFIER";

        case "Microsoft.SqlServer.Types.SqlGeometry":
        case "System.Data.Spatial.DbGeometry":
          return "GEOMETRY";

        default:
          throw new Exception(type + " not implemented.");
      }
    }
  }
}
