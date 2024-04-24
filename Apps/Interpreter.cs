using Parser;
using FileManager;
using Interpreter;
using CustomCommandManager;
using System.Text;
using System.IO;

namespace Apps;
public class Interpreter : App
{
    private List<string> loopDetection = new List<string>();

    public string WorkingDirectory = "";


    public Stack<ulong> LastMain = new Stack<ulong>();
    public Stack<ulong> LastStorage = new Stack<ulong>();

    public Interpreter()
    {
    }

    private protected override void exec()
    {
        string inputFile = "";
        while (inputFile == "")
        {
            string answer = Ask("File path:", ConsoleColor.Cyan);
            string path = answer;            

            //Treat as path
            try
            {
                FileManager.FileManager fm = new FileManager.FileManager(path + ".smpl", "");
                string cont = fm.GetContent();
            }
            catch
            {
                continue;
            }
            inputFile = path;
        }

        WriteLine();
        WriteLine($"Starting program {Path.GetFileName(inputFile)}", CommandInfoColor);
        WriteLine();

        Interpret(inputFile, new Stack<ulong>());

        WriteLine();
        WriteLine();
        WriteLine($"Program {Path.GetFileName(inputFile)} ended", CommandInfoColor);
        WriteLine();
    }

    public override Result HandleCustomCommand(string command)
    {
        return new InterpreterCommandManager().Handle(command, this);
    }
    public override string GetCommandHandlerName()
    {
        return new InterpreterCommandManager().GetName();
    }


    public override string GetShortName()
    {
        return "Itptr";
    }

    public Stack<ulong> Interpret(string filePath, Stack<ulong> inStack, bool isRoot = true)
    {
        if (isRoot)
        {
            WorkingDirectory = null;
        }

        if (loopDetection.Contains(filePath))
        {
            if (!isRoot)
            {
                Error(new Executer.ExecuterException($"File loop detected when interpreting file {filePath}"));
            }
            return new Stack<ulong>();
        }
        
        loopDetection.Add(filePath);

        FileManager.FileManager manager = new FileManager.FileManager(filePath + (filePath.EndsWith(".smpl") ? "" : ".smpl"), WorkingDirectory);
        this.WorkingDirectory = manager.WorkingDirectory;

        Parser.Parser parser = new Parser.Parser(manager, this);
        Executer executer = new Executer(Path.GetFileName(filePath), inStack, this);
        Stack<ulong>[] stacks = executer.Execute(parser.Parse(), parser.GetUsedDirectories());
        Stack<ulong> main = stacks[0];
        Stack<ulong> storage = stacks[1];
        loopDetection.Remove(filePath);
        LastMain = main;
        LastStorage = storage;
        return storage;
    }

    public class InterpreterCommandManager : GlobalCommandManager<Interpreter>
    {
        public CustomCommand<Interpreter> PrintLastStack;


        protected override void init()
        {
            base.init();

            PrintLastStack = AddCustomCommand(["stack", "print last stack", "print stack", "last stack", "print last"], (args, app) =>
            {
                app.WriteLine("Printing last stack", App.CommandInfoColor);

                app.WriteLine("\tMain stack: (top -> bottom)", ConsoleColor.Blue);
                ulong i = 0;
                foreach (ulong item in app.LastMain.Reverse())
                {
                    app.WriteLine($"\t\t{i}: {item}", ConsoleColor.Green);
                    i++;
                }

                app.WriteLine();

                app.WriteLine("\tStorage stack: (top -> bottom)", ConsoleColor.Blue);
                i = 0;
                foreach (ulong item in app.LastStorage.Reverse())
                {
                    app.WriteLine($"\t\t{i}: {item}", ConsoleColor.Green);
                    i++;
                }
                return new string[] { };
            }, (args) => $"Succesfully printed last stack");
        }
    }
}
