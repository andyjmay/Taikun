using System.Collections.Generic;

namespace Taikun {
  public interface IProjectManager {
    IEnumerable<IProject> GetAllProjects();
    IProject GetProject(string databaseName);
    IProject CreateProject(IProject project);    
    IProject UpdateProject(IProject project);
    void DeleteProject(IProject project);

    void CreateProjectTable(IProject project, IProjectTable projectTable);
    void DeleteProjectTable(IProject project, IProjectTable projectTable);
  }
}