using System.Linq;

namespace Taikun {
  public interface IProjectManager {
    IQueryable<IProject> GetAllProjects();
    IProject CreateProject(IProject project);    
    IProject UpdateProject(IProject project);
    void DeleteProject(IProject project);
  }
}
