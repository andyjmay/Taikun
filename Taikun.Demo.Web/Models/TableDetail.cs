using System.Collections.Generic;

namespace Taikun.Demo.Web.Models {
  public class TableDetail {
    public IEnumerable<string> PrimaryKeys { get; set; }
    public IEnumerable<Column> Columns { get; set; }
  }
}