using CustomCommandManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomCommandManager;

public abstract class CustomCommandManager<T> where T : App
{    

    public CustomCommandManager()
    {
        init();
    }

    protected abstract void init();

    public Result Handle(string command, T app)
    {
        if (command == null || command == "")
            return Result.NOT_A_COMMAND;

        command = Regex.Replace(command, @"^\s+", "");
        if (!command.StartsWith("$"))
        {
            return Result.NOT_A_COMMAND;
        }
        command = command.Substring(1);
        command = command.Trim();

        //Handle commands

        try
        {
            Result result = Result.UNKNOWN_COMMAND;

            foreach (CustomCommand<T> com in customCommands)
            {
                Result r = tryCommand(command, com, app);
                if (r == Result.SUCCESS)
                {
                    return Result.SUCCESS;
                }

                if ((int)r < (int)result)
                {
                    result = r;
                }
            }
            return result;
        }
        catch (App.AppException e)
        {
            if (e is App.CommandException<T>)
            {
                app.WriteLine();
                app.WriteLine($"{e}", ConsoleColor.Red);
                return Result.COMMAND_ERROR;
            }

            throw;
        }

    }

    private Result tryCommand(string str, CustomCommand<T> command, T app)
    {
        string[] split = str.Split(' ');

        foreach (string[] alias in command.GetAliases())
        {
            bool br = false;

            if (alias.Length > split.Length)
                br = true;

            string parameters = "";
            for (int i = 0; i < split.Length; i++)
            {
                if (i < alias.Length)
                {
                    if (!App.CanAutoFill(split[i], alias[i]))
                    {
                        br = true;
                    }
                }
                else
                {
                    parameters += split[i] + " ";
                }
            }

            if (br)
                continue;

            parameters = parameters.Trim();


                return command.Run(parameters, app);
            
        }

        return Result.UNKNOWN_COMMAND;
    }



    protected List<CustomCommand<T>> customCommands = new List<CustomCommand<T>>();

    protected CustomCommand<T> AddCustomCommand(string condition, Func<string[], T, string[]> action, Func<string[], string> successMessage = null)
    {
        CustomCommand<T> command = new CustomCommand<T>(condition, action, successMessage);
        this.customCommands.Add(command);
        return command;
    }

    protected CustomCommand<T> AddCustomCommand(string[] aliases, Func<string[], T, string[]> action, Func<string[], string> successMessage = null)
    {
        CustomCommand<T> command = new CustomCommand<T>(aliases, action, successMessage);
        this.customCommands.Add(command);
        return command;
    }



    public virtual string GetName()
    {
        return this.GetType().Name;
    }
}

public enum Result
{
    SUCCESS,
    COMMAND_ERROR,
    UNKNOWN_COMMAND,
    NOT_A_COMMAND
}
