using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomCommandManager;

public class CustomCommand<T> where T : App
{
    private string[] aliases;
    private Func<string[], T, string[]> action;
    private Func<string[], string> successMessage;

    public (string[], string) HelpMessage;

    public CustomCommand(string condition, (string[], string) HelpMessage, Func<string[], T, string[]> action, Func<string[], string> successMessage = null)
    {
        this.aliases = [condition];
        this.action = action;
        this.successMessage = successMessage;
        this.HelpMessage = HelpMessage;
    }

    public CustomCommand(string[] aliases, (string[], string) HelpMessage, Func<string[], T, string[]> action, Func<string[], string> successMessage = null)
    {
        this.aliases = aliases;
        this.action = action;
        this.successMessage = successMessage;
        this.HelpMessage = HelpMessage;
    }

    public string GetName()
    {
        return this.aliases[0];
    }

    public CustomCommandManager.Result Run(string str, T app)
    {
            
        List<string> split = new List<string>();

        for (int i = 0; i < str.Split(' ').Length; i++)
        {
            string s = str.Split(' ')[i];
            if (string.IsNullOrEmpty(s))
            {
                continue;
            }

            split.Add(Regex.Replace(s, @"\s", ""));
        }
        string[] result = action(split.ToArray(), app);
        if (result == null)
        {
            return CustomCommandManager.Result.COMMAND_ERROR;
        }

        if (successMessage != null)
            app.WriteLine(successMessage(result), App.CommandInfoColor);


        return CustomCommandManager.Result.SUCCESS;

    }



    public string[][] GetAliases()
    {
        string[][] result = new string[aliases.Length][];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = aliases[i].Split(' ');
        }

        return result;
    }
}
