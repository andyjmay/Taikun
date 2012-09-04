namespace Taikun.Demo.WPF.Models {
  public class SqlServerTableColumn {
    public string Name { get; set; }
    public string Type { get; set; }
    public int? Length { get; set; }
    public bool AllowNulls { get; set; }
    public bool PrimaryKey { get; set; }
    public bool Identity { get; set; }
  }
}
