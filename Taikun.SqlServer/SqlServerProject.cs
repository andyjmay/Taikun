namespace Taikun.SqlServer {
  public class SqlServerProject : IProject {
    public int Id { get; set; }
    public string DatabaseName { get; set; }
    public string Description { get; set; }
  }
}
