
using CustomCommandManager;
using System.Drawing;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

public abstract class App
{
    private AppException quitException = new QuitException();

    public readonly static ConsoleColor AppInfoColor = ConsoleColor.Yellow;
    public readonly static ConsoleColor CommandInfoColor = ConsoleColor.Magenta;

    private bool isFirstLineInApp = true;

    public void Execute()
    {
        isFirstLineInApp = true;
        try
        {
            while (true)
            {
                exec();
            }
        }
        catch (AppException e)
        {
            if (e.isError)
            {

            }
        }
    }

    private protected abstract void exec();


    private string getPrompt(bool isRead = false)
    {
        string prompt = "";
        if (string.IsNullOrEmpty(this.GetShortName()))
            return " ";

        if (isRead)
            prompt = $" ->";

        string formattedName = $" [{this.GetShortName()}]";

        if (!isFirstLineInApp)
            return "".PadRight(formattedName.Length, ' ') + prompt + " ";
        return $"{formattedName}{prompt}: ";
    }


    public static RootApp Root = new RootApp();



    public void Quit()
    {
        if (this == Root)
        {
            Root.WriteLine("--No application is currently open--", App.CommandInfoColor);
            return;
        }

        if (!string.IsNullOrEmpty(quitMessage()))
            Root.WriteLine($"[{quitMessage()}]", ConsoleColor.Yellow);

        throw quitException;
    }

    private protected virtual string quitMessage()
    {
        return $"Application {this.GetType().Name} closed";
    }

    public virtual string GetDisplayName()
    {
        return $"{this.GetType().Name}";
    }



    private const int shortNameLength = 5;
    public virtual string GetShortName()
    {
        string name = this.GetDisplayName();
        if (name.Length <= shortNameLength) 
            return name;
        return $"{name.Substring(0, shortNameLength)}{(name.Length > shortNameLength ? "..." : "")}";
    }


    //Util
    public virtual Result HandleCustomCommand(string command)
    {
        return new GlobalCommandManager<RootApp>().Handle(command, Root);
    }
    public virtual string GetCommandHandlerName()
    {
        return new GlobalCommandManager<RootApp>().GetName();
    }



    public string Ask(string firstMessage, string message, string[] options, ConsoleColor color = ConsoleColor.White, bool canBeNull = false)
    {
        string val = "";


        //The first message
        val = AskOnce(firstMessage, options, color, canBeNull);

        //The other message
        while (val == "" && !canBeNull)
        {
            val = AskOnce(message, options, color, canBeNull);
        }

        return val;
    }

    public string Ask(string message, string[] options, ConsoleColor color = ConsoleColor.White, bool canBeNull = false)
    {
        return Ask(message, message, options, color, canBeNull);
    }

    public string Ask(string message, ConsoleColor color = ConsoleColor.White, bool canBeNull = false)
    {
        return Ask(message, new string[] { }, color, canBeNull);
    }

    public string Ask(string message, params string[] options)
    {
        return Ask(message, options, ConsoleColor.White, false);
    }



    public string AskOnce(string message, string[] options, ConsoleColor color = ConsoleColor.White, bool canBeNull = false)
    {
        string mes = "";
        if (options.Length > 0)
        {
            mes = " (";
            for (int i = 0; i < options.Length - 1; i++)
            {
                mes += options[i] + " / ";
            }
            mes += options.Last() + ")";
        }


        WriteLine(message + mes, color);
        string text = ReadLine(ConsoleColor.White);

        string matched = AutoFill(text, options);

        if (text == null || text == "")
        {
            if (canBeNull)
                return "";
            else
                WriteLine("--Invalid answer--", ConsoleColor.DarkMagenta);
        }

        switch (HandleCustomCommand(text))
        {
            case CustomCommandManager.Result.SUCCESS:
                return text;

            case CustomCommandManager.Result.UNKNOWN_COMMAND:
                WriteLine("--Unrecognized command, please try again or use $? for help--", ConsoleColor.DarkMagenta);
                break;

            case CustomCommandManager.Result.COMMAND_ERROR:
                break;
        }

        if (options.Length == 0)
        {
            return text;
        }

        return matched;
    }



    public static string AutoFill(string text, params string[] options)
    {


        if (options.Length == 0)
        {
            return text;
        }

        List<int> matches = new List<int>();
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].StartsWith(text))
            {
                matches.Add(i);
            }
        }

        if (matches.Count == 1)
        {
            return options[matches[0]];
        }
        return "";
    }

    public static bool CanAutoFill(string text, string other)
    {
        
        if (other.StartsWith(text))
        {
            return true;
        }
        
        return false;
    }



    private static bool isNewLine = true;
    public void Write(object message, ConsoleColor color = ConsoleColor.White)
    {
        if (isNewLine)
        {
            writePrompt();
        }

        Console.ForegroundColor = color;

        Console.Write(message.ToString());
        if (message.ToString().Last() == '\n')
        {
            isNewLine = true;
        }
        else
        {
            isNewLine = false;
        }
        Console.ResetColor();
    }

    public void WriteLine(object message = null, ConsoleColor color = ConsoleColor.White)
    {
        if (isNewLine)
            writePrompt();

        Console.ForegroundColor = color;
        Console.WriteLine(message == null ? "" : message.ToString());
        isNewLine = true;
        Console.ResetColor();
    }

    public void Error(AppException exception)
    {
        WriteLine(exception.message == null ? "" : exception.message.ToString(), ConsoleColor.Red);
        throw exception;
    }


    public string? ReadLine(ConsoleColor color = ConsoleColor.White)
    {
        if (isNewLine)
        {
            writePrompt(true);
        }

        isNewLine = true;
        Console.ForegroundColor = color;
        string? input = Console.ReadLine();
        Console.ResetColor();
        return input;
    }


    private void writePrompt(bool isRead = false)
    {
        Console.ForegroundColor = AppInfoColor;
        Console.Write(getPrompt(isRead));
        Console.ResetColor();
        isFirstLineInApp = false;
    }

    public class RootApp : App
    {
        private protected override void exec()
        {
            
        }

        public override string GetDisplayName()
        {
            return "";
        }
    }

    public abstract class AppException : Exception
    {
        public bool isError;
        public string message;

        public override string ToString()
        {
            return (isError ? "Application exception: " : "Application message: ") + message;
        }

        
    }

    public class CommandException<T> : AppException where T : App
    {
        public CustomCommand<T> sourceCommand;
        public CommandException(string message, CustomCommand<T> sourceCommand)
        {
            this.sourceCommand = sourceCommand;
            this.message = message;
            this.isError = true;
        }

        public override string ToString()
        {
            return $"Command error at command \"{sourceCommand.GetName()}\" with message: " + message;
        }
    }

    public class QuitException : AppException
    {
        public QuitException(string message = "Quit application")
        {
            this.message = message;
            this.isError = false;
        }

    }
}
