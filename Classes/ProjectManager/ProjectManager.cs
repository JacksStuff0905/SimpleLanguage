using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ProjectManager;

public static class ProjectManager
{
    public static string ProjectNameFile = "projects.smpldat";
    public static string LibrariesDirectory = "libs/";

    public static char ProjectChar = '%';

    private static Dictionary<string, string> savedProjects = new Dictionary<string, string>();

    public static Dictionary<string, string> GetProjects()
    {
        updateDictionary();
        return new Dictionary<string, string>(savedProjects);
    }

    public static string GetPath(string projectName)
    {
        updateDictionary();
        return savedProjects[projectName];
    }

    public static void AddProject(string Name, string Path)
    {
        updateDictionary();
        
        if (savedProjects.ContainsKey(Name))
        {
            savedProjects[Name] = fix(Path);
        }
        else
        {
            savedProjects.Add(Name, fix(Path));
        }
        updateFile();
    }

    public static void RemoveProject(string Name)
    {
        updateDictionary();
        savedProjects.Remove(Name);
        updateFile();
    }

    public static void ClearProjects()
    {
        savedProjects.Clear();
        updateFile();
    }

    private static void updateFile()
    {
        fixProjects();
        string newText = JsonSerializer.Serialize(savedProjects);
        File.WriteAllText(ProjectNameFile, newText);
    }

    private static void updateDictionary()
    {
        try
        {
            string text = File.ReadAllText(ProjectNameFile);
            savedProjects = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
            fixProjects();
        }
        catch
        {
            return;
        }
    }

    private static string fix(string Path)
    {
        string path = Regex.Replace(Path, @"\/", "\\");
        if (!path.EndsWith('\\'))
        {
            path += "\\";
        }
        return path;
    }

    private static void fixProjects()
    {
        foreach (KeyValuePair<string, string> pair in savedProjects)
        {
            savedProjects[pair.Key] = fix(pair.Value);
        }
    }


    public static string ReplaceProjectsInPath(string inPath)
    {
        

        string[] split = inPath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
        string path = "";
        foreach (string s in split)
        {
            if (s.StartsWith(ProjectChar))
            {
                try
                {
                    path = Path.Combine(path, GetPath(s.Substring(1)));
                }
                catch
                {
                    App.Root.Error(new ProjectManagerException($"Invalid project name: {s.Substring(1)}"));
                }
            }
            else
            {
                path = Path.Combine(path, s);
            }
        }

        return path;
    }



    public class ProjectManagerException : App.AppException
    {
        public ProjectManagerException(string message)
        {
            this.isError = true;
            this.message = $"Project Manager Exception: {message}";
        }
    }
}

