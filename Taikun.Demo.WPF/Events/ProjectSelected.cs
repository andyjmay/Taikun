using Taikun.SqlServer;
using Taikun.Demo.WPF.Models;
namespace Taikun.Demo.WPF.Events {
  public class ProjectSelected {
    public ProjectSelected(IProject project) {
      Project = project;
    }
      
    public IProject Project { get; private set; }
  }
}
