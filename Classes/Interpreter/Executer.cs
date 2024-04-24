using Parser;
using System;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.RegularExpressions;
namespace Interpreter;
public class Executer
{

    //The two stacks
    public Stack<ulong> Main { get; private set; }
    public Stack<ulong> Storage { get; private set; }

    public Stack<ulong> inStorage { get; private set; } = new Stack<ulong>();


private List<Command> commands;
    private ulong index = 0;
    private string name;
    private Apps.Interpreter interpreter;
    private (string, string)[] usedFiles;

    public Executer(string name, List<Command> commands, Apps.Interpreter interpreter)
    {
        this.name = name;
        this.commands = commands;
        this.Main = new Stack<ulong>();
        this.Storage = new Stack<ulong>();
        this.interpreter = interpreter;
    }


    public Executer(string name, Apps.Interpreter interpreter)
    {
        this.name = name;
        this.commands = Commands.ALL;
        this.Main = new Stack<ulong>();
        this.Storage = new Stack<ulong>();
        this.interpreter = interpreter;
    }

    public Executer(string name, Stack<ulong> inStack, List<Command> commands, Apps.Interpreter interpreter)
    {
        this.name = name;
        this.commands = commands;
        this.Main = new Stack<ulong>();
        this.Storage = new Stack<ulong>();
        this.inStorage = inStack;
        this.interpreter = interpreter;
    }

    public Executer(string name, Stack<ulong> inStack, Apps.Interpreter interpreter)
    {
        this.name = name;
        this.commands = Commands.ALL;
        this.Main = new Stack<ulong>();
        this.Storage = new Stack<ulong>();
        this.inStorage = inStack;
        this.interpreter = interpreter;
    }

    public Stack<ulong>[] Execute(Parser.Line[] lines, (string, string)[] usedFiles)
    {
        this.usedFiles = usedFiles;
        index = 0;
        try
        {
            executeLine(lines);
        }
        catch (HaltException e)
        {

        }

        return [Main, Storage];
    }


    private void executeLine(Line[] lines)
    {
        if (lines.Length <= (int)index)
            return;

        
        Parser.Line line = lines[index];


        if (line.Command == "")
        {
            index += 1;
            executeLine(lines);
        }

        if (line.Command.Any(char.IsLower))
        {
            Halt("ERROR: Commands must be upper case - " + line.Command);
            return;
        }

        //Error checking
        Command command = match(line);

        if (line.Command == "" || line.Command == null)
        {
            return;
        }

        if (command == null)
        {
            Halt("ERROR: Unknown command - " + line.Command);
            return;
        }



        
        //Performing commands
        if (line.File == "")
            command.Execute(this, line.Parameters);
        else
            command.Execute(this, Commands.OPEN(line.File, line.Parameters, this));

        //Next line
        index += 1;
        executeLine(lines);
    }


    private Command match(Parser.Line line)
    {
        foreach (Command command in commands)
        {
            if (command.Name == line.Command)
            {
                return command;
            }
        }

        return null;
    }

    private Command parseFile(string name)
    {
        if (name.Any(char.IsLower))
        {
            return null;
        }
        return null;//Commands.OPEN(name);
    }

    public static char GetCharFromULong(ulong value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        return Encoding.UTF8.GetString(bytes)[0];
    }


    public void Halt(string error = "")
    {

        interpreter.WriteLine();
        interpreter.WriteLine($"Program {name} halted at line {index + 1}", error != "" ? ConsoleColor.Red : ConsoleColor.White);

        if (error != "")
        {
            interpreter.WriteLine(error, ConsoleColor.Red);
        }

        throw new HaltException("");
    }

    public void Print(object message)
    {
        interpreter.Write(message.ToString());
    }

    public void PrintChar(ulong num)
    {
        if (num == 0)
            return;


        interpreter.Write(GetCharFromULong(num), ConsoleColor.DarkYellow);
    }

    public void PrintString(ulong[] parameters)
    {
        for (int i = 0; i < parameters.Length; i++)
        {
            ulong num = parameters[i];
            if (num == 0)
                return;

            byte code = (byte)num;

            interpreter.Write(Encoding.UTF8.GetString(new byte[] { code }), ConsoleColor.Green);
        }
    }

    public ulong[] Input()
    {
        List<ulong> parameters = new List<ulong>();
        string text = interpreter.Ask("", App.CommandInfoColor);
        

        parameters.AddRange(Parser.Parser.ParseText(text).Item1);
        

        return parameters.ToArray();
    }

    public void Jump(ulong index)
    {
        this.index = index - 2;
    }

    public ulong[] Open(string fileName, Stack<ulong> bonusStack)
    {
        string path = fileName;
        for (int i = 0; i < usedFiles.Length; i++)
        {
            if (usedFiles[i].Item2.ToUpper() == fileName.ToUpper())
            {
                path = usedFiles[i].Item1;
            }
        }
        Stack<ulong> newStack = new Stack<ulong>(this.Storage.Reverse());
        Stack<ulong> bStack = new Stack<ulong>(bonusStack);
        for (int i = 0; i < bonusStack.Count; i++)
        {
            newStack.Push(bStack.Pop());
        }
        bStack = new Stack<ulong>(this.inStorage.Reverse());
        for (int i = 0; i < this.inStorage.Count; i++)
        {
            newStack.Push(bStack.Pop());
        }

        Stack<ulong> s = new Stack<ulong>();
        while (newStack.Count > 0)
        {
            s.Push(newStack.Pop());
        }

        Stack<ulong> stack = interpreter.Interpret(path, s, false);
        return stack.Reverse().ToArray();
    }



    //Stack
    public void Push(ulong value)
    {
        this.Main.Push(value);
    }

    public ulong Pop()
    {
        if (this.Main.Count > 0)
        {
            return this.Main.Pop();
        }
        return 0;
    }

    public ulong Peek()
    {
        if (this.Main.Count > 0)
        {
            return this.Main.Peek();
        }
        return 0;
    }

    public void Set(ulong[] input)
    {
        this.Main = new Stack<ulong>(input);
    }

    public void Put()
    {
        if (IsMain())
        {
            this.Storage.Push(this.Pop());
        }
    }

    public void Get()
    {
        if (this.Storage.Count > 0)
        {
            this.Push(this.Storage.Pop());
        }
        else if (this.inStorage.Count > 0)
        {
            this.Push(this.inStorage.Pop());
        }
        else
        {
            this.Push(0);
        }
    }

    public bool IsMain()
    {
        return Main.Count > 0;
    }

    public bool IsStorage()
    {
        return Storage.Count > 0 || inStorage.Count > 0;
    }

    public void Swap()
    {
        Stack<ulong> a = new Stack<ulong>(this.Main.Reverse());
        Stack<ulong> b = new Stack<ulong>(this.Storage.Reverse());

        this.Storage = a;
        this.Main = b;
    }

    public void ClearMain()
    {
        this.Main = new Stack<ulong>();
    }

    public void ClearStorage()
    {
        this.Storage = new Stack<ulong>();
    }


    public class Command
    {
        public string Name { get; private set; }
        //private readonly Format format;

        private Action<Executer, ulong[]> action;

        public Command(string Name, Action<Executer, ulong[]> action)
        {
            //this.format = format;
            this.Name = Name;
            this.action = action;
        }

        public void Execute(Executer executer, ulong[] parameters)
        {
            action.Invoke(executer, parameters);
        }
        
    }



    public class ExecuterException : App.AppException
    {
        public ExecuterException(string message)
        {
            this.isError = true;
            this.message = $"Executer exception: {message}";
        }
    }

    public class HaltException : ExecuterException
    {
        public HaltException(string message) : base(message)
        {
            this.isError = false;
            this.message = message;
        }
    }
}
