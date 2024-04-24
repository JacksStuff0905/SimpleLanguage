using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomCommandManager;

public class GlobalCommandManager<T> : CustomCommandManager<T> where T : App
{


    public CustomCommand<T> AddProject;

    public CustomCommand<T> DeleteProject;

    public CustomCommand<T> QuitApp;

    public CustomCommand<T> AppInfo;



    protected override void init()
    {

        AddProject = AddCustomCommand(["project add", "project +"], (args, app) =>
        {

            if (args.Length > 1 && Regex.Match(args[0].Substring(1), @"[\+\\\/\-\%]").Success)
            {
                app.Error(new App.CommandException<T>("Invalid project name, contains prohibited characters", AddProject));
            }
            if (args.Length != 2)
            {
                app.Error(new App.CommandException<T>("Invalid parameter count, expected 2, received " + args.Length, AddProject));
            }

            ProjectManager.ProjectManager.AddProject(args[0], args[1]);

            return [args[0], args[1]];
        }, (args) => $"Project {args[0]} with path {args[1]} added succesfully");

        DeleteProject = AddCustomCommand(["project delete", "project -", "project remove"], (args, app) =>
        {
            if (args.Length != 1)
            {
                app.Error(new App.CommandException<T>("Invalid parameter count, expected 1, received " + args.Length, DeleteProject));
                return null;
            }
            string confirm = app.Ask($"Are you sure you want to delete project {args[0]}?", ["yes", "no"], ConsoleColor.DarkYellow);
            if (confirm == "yes")
                ProjectManager.ProjectManager.RemoveProject(args[0]);

            return new string[] { args[0], confirm };
        }, (args) => args[1] == "yes"? $"Project {args[0]} succesfully deleted" : "");

        QuitApp = AddCustomCommand(["quit", "exit", "end", "close"], (args, app) =>
        {
            app.Quit();
            return [];
        }, (args) => "");

        AppInfo = AddCustomCommand(["app info", "info"], (args, app) =>
        {
            string displayName = app.GetDisplayName();
            string shortName = app.GetShortName();
            string className = app.GetType().Name;
            string handlerName = app.GetCommandHandlerName();

            displayName = string.IsNullOrEmpty(displayName) ? "-" : displayName;
            shortName = string.IsNullOrEmpty(shortName) ? "-" : shortName;
            className = string.IsNullOrEmpty(className) ? "-" : className;
            handlerName = string.IsNullOrEmpty(handlerName) ? "-" : handlerName;

            app.WriteLine("Printing current app info", App.CommandInfoColor);
            app.Write($"\tApp display name: ", App.AppInfoColor);               app.WriteLine(displayName, ConsoleColor.Blue);
            app.Write($"\tApp short name: ", App.AppInfoColor);                 app.WriteLine(shortName, ConsoleColor.Blue);
            app.Write($"\tApp class name: ", App.AppInfoColor);                 app.WriteLine(className, ConsoleColor.Blue);
            app.Write($"\tApp command handler name: ", App.AppInfoColor);       app.WriteLine(handlerName, ConsoleColor.Blue);
            app.WriteLine("Finished printing current app info", App.CommandInfoColor);

            return [];
        }, (args) => "");

    }
}
