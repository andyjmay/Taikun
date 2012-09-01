namespace Taikun.Demo.WPF.Events {
  public class ProjectCreated {
    public ProjectCreated(IProject project) {
      Project = project;
    }
    public IProject Project { get; private set; }
  }
}
