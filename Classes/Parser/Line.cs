using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Parser;
public class Line
{
    public string Command { get; private set; }
    public ulong[] Parameters { get; private set; }

    public string File { get; private set; }

    public string Whole { get; private set; }

    public Line(string Command, ulong[] Parameters, string File, string Whole)
    {
        this.Command = Command;
        this.Parameters = Parameters;
        this.File = File;
        this.Whole = Whole;
    }
}
