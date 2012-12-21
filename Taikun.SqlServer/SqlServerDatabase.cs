namespace Taikun.SqlServer {
  public class SqlServerDatabase : IDatabase {
    public int Id { get; set; }
    public string DatabaseName { get; set; }
    public string Description { get; set; }
  }
}
