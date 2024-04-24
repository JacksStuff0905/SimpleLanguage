
using Interpreter;
using System.IO;

namespace FileManager;
public class FileManager
{
    public string Path { get; private set; }

    public string WorkingDirectory;

    public static readonly char rootDirectorySymbol = '&';


    public FileManager(string Path, string WorkingDirectory)
    {
        this.WorkingDirectory = WorkingDirectory;


        if (WorkingDirectory == null)
        {
            this.WorkingDirectory = System.IO.Path.GetDirectoryName(ProjectManager.ProjectManager.ReplaceProjectsInPath(Path));
            this.Path = ProjectManager.ProjectManager.ReplaceProjectsInPath(Path);
        }
        else
        {
            this.Path = System.IO.Path.Combine(WorkingDirectory, ProjectManager.ProjectManager.ReplaceProjectsInPath(Path));
        }

        if (ProjectManager.ProjectManager.ReplaceProjectsInPath(Path).StartsWith(rootDirectorySymbol))
        {
            this.Path = ProjectManager.ProjectManager.ReplaceProjectsInPath(Path).Trim().Substring(1);
            this.WorkingDirectory = "";
        }
    }

    public string GetContent()
    {
        return File.ReadAllText(System.IO.Path.Combine(WorkingDirectory, Path));
    }

    public void WriteContent(string content)
    {
        File.WriteAllText(System.IO.Path.Combine(WorkingDirectory, Path), content);
    }


    public string[] GetFiles()
    {
        return Directory.GetFiles(this.Path);
    }
}