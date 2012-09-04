namespace Taikun.Demo.WPF.Events {
  public class ProjectTableSelected {
    public ProjectTableSelected(IProjectTable projectTable) {
      ProjectTable = projectTable;
    }

    public IProjectTable ProjectTable { get; private set; }
  }
}
