public static class Program
{
    public static void Main(string[] args)
    {
        Console.Title = "Simple Language";
        string specialCommand = App.Root.Ask(firstMessage: "==============================================================================\n" +
                       " Welcome to Simple Language! Press enter to continue or write a config command:\n" +
                       " ==============================================================================", message: "Press enter to continue or write a config command:", options: [], color: ConsoleColor.Yellow, canBeNull: true);

        while (true)
        {
            
            
            string mode = App.Root.Ask("Mode:", ["interpret", "compile"], ConsoleColor.Yellow);
            switch (mode)
            {
                case "interpret":
                    handleInterpreter();
                    break;

                case "compile":
                    handleCompiler();
                    break;
            }
            

        }
    }

    

    private static void handleInterpreter()
    {
        Apps.Interpreter interpreter = new Apps.Interpreter();
        interpreter.Execute();
    }

    private static void handleCompiler()
    {
        Apps.Compiler compiler = new Apps.Compiler();
        compiler.Execute();
    }


    



    
}