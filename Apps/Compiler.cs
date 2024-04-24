namespace Apps;

public class Compiler : App
{
    private protected override void exec()
    {
        Error(new App.QuitException("The compiler has not been implemented yet, please use the interpreter"));
        WriteLine();
    }

    public override string GetShortName()
    {
        return "Complr";
    }
}
