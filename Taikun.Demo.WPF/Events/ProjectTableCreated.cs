namespace Taikun.Demo.WPF.Events {
  public class ProjectTableCreated {
    public ProjectTableCreated(IProjectTable projectTable) {
      ProjectTable = projectTable;
    }

    public IProjectTable ProjectTable { get; private set; }
  }
}
