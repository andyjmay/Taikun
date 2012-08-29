using System.Collections.Generic;

namespace Taikun {
  public interface IProjectManager {
    IEnumerable<IProject> GetAllProjects();
    IProject GetProject(string databaseName);
    IProject CreateProject(IProject project);    
    IProject UpdateProject(IProject project);
    void DeleteProject(IProject project);
  }
}
