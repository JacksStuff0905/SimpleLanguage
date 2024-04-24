using System.Text;
using Command = Interpreter.Executer.Command;

namespace Interpreter;
public static class Commands
{
    static Commands()
    {
        PUSH = create("PUSH", (executer, parameters) =>
        {
            for (int i = 0; i < parameters.Length; i++)
                executer.Push(parameters[parameters.Length - i - 1]);
        });
        POP = create("POP", (executer, parameters) => executer.Pop());
        SET = create("SET", (executer, parameters) => executer.Set(parameters));
        COPY = create("COPY", (executer, parameters) => executer.Push(executer.Peek()));
        PUT = create("PUT", (executer, parameters) => 
        {
            if (parameters.Length == 0)
            {
                executer.Put();
                return;
            }
            for (int i = 0; i < parameters.Length; i++)
                executer.Put();
        });
        GET = create("GET", (executer, parameters) => 
        {
            if (parameters.Length == 0)
            {
                executer.Get();
                return;
            }
            for (ulong i = 0; i < parameters[0]; i++)
                executer.Get();
        });
        SWAP = create("SWAP", (executer, parameters) => executer.Swap());
        CLEARMAIN = create("CLEARMAIN", (executer, parameters) => executer.ClearMain());
        CLEARSTOR = create("CLEARSTOR", (executer, parameters) => executer.ClearStorage());


        ADD = create("ADD", (executer, parameters) =>
        {
            ulong a = executer.Pop();
            ulong b = executer.Pop();
            executer.Push(a + b);
        });
        SUB = create("SUB", (executer, parameters) =>
        {
            ulong a = executer.Pop();
            ulong b = executer.Pop();
            executer.Push(b - a);
        });
        MUL = create("MUL", (executer, parameters) =>
        {
            ulong a = executer.Pop();
            ulong b = executer.Pop();
            executer.Push(a * b);
        });
        DIV = create("DIV", (executer, parameters) =>
        {
            ulong a = executer.Pop();
            ulong b = executer.Pop();
            executer.Push(b / a);
        });


        PRINT = create("PRINT", (executer, parameters) =>
        {
            if (parameters.Length == 0)
            {
                executer.Print(executer.Peek());
                return;
            }
            executer.Print(parameters[0]);
        });
        PRINTC = create("PRINTC", (executer, parameters) =>
        {
            if (parameters.Length == 0)
                executer.PrintChar(executer.Peek());
            else
                executer.PrintChar(parameters[0]);
        });
        /*PRINTS = create("PRINTS", (executer, parameters) =>
        {
            executer.PrintString(parameters); 
        });*/
        //PRINTL = create("PRINTL", (executer, parameters) => executer.Print("\n"));
        INPUT = create("INPUT", (executer, parameters) =>
        {
            foreach (ulong tmp in executer.Input().Reverse())
                executer.Push(tmp);
        });


        HALT = create("HALT", (executer, parameters) => executer.Halt());
        JMPEQ0 = create("JMPEQ0", (executer, parameters) =>
        {
            if (executer.Peek() == 0)
                executer.Jump(parameters[0]);
        });
        JMPGT0 = create("JMPGT0", (executer, parameters) =>
        {
            if (executer.Peek() > 0)
                executer.Jump(parameters[0]);
        });
        JMP = create("JMP", (executer, parameters) =>
        {
            executer.Jump(parameters[0]);
        });
        JMPMAIN = create("JMPMAIN", (executer, parameters) =>
        {
            if (executer.IsMain())
                executer.Jump(parameters[0]);
        });
        JMPNOMAIN = create("JMPNOMAIN", (executer, parameters) =>
        {
            if (!executer.IsMain())
                executer.Jump(parameters[0]);
        });
        JMPSTOR = create("JMPSTOR", (executer, parameters) =>
        {
            if (executer.IsStorage())
                executer.Jump(parameters[0]);
        });
        JMPNOSTOR = create("JMPNOSTOR", (executer, parameters) =>
        {
            if (!executer.IsStorage())
                executer.Jump(parameters[0]);
        });
        create("OPEN", (executer, parameters) =>
        {
            string fileName = "";
            for (int i = 0; i < parameters.Length; i++)
            {
                fileName += Executer.GetCharFromULong(parameters[i]);
            }
            foreach (ulong value in executer.Open(fileName, new Stack<ulong>()))
            {
                executer.Push(value);
                executer.Put();
            }
        });
    }

    //Stack managment
    public static Command PUSH;
    public static Command POP;
    public static Command SET;
    public static Command COPY;
    public static Command PUT;
    public static Command GET;
    public static Command SWAP;
    public static Command CLEARMAIN;
    public static Command CLEARSTOR;

    //Math
    public static Command ADD;
    public static Command SUB;
    public static Command MUL;
    public static Command DIV;

    //User Interface
    public static Command PRINT;
    public static Command PRINTC;
    public static Command INPUT;

    //Flow control
    public static Command HALT;
    public static Command JMPEQ0;
    public static Command JMPGT0;
    public static Command JMP;

    public static Command JMPMAIN;
    public static Command JMPNOMAIN;
    public static Command JMPSTOR;
    public static Command JMPNOSTOR;
    public static ulong[] OPEN(string fileName, ulong[] parameters, Executer executer)
    {
        return executer.Open(fileName, new Stack<ulong>(parameters));
    }


    private static List<Command> _all = new List<Command>();
    public static List<Command> ALL {
        get {
            return _all; 
        }
    }
    private static Command create(string Name, Action<Executer, ulong[]> action)
    {
        Command cmd = new Command(Name, action);
        if (_all == null)
        {
            _all = new List<Command>();
        }
        _all.Add(cmd);
        return cmd;
    }
}
