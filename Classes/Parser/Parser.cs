using Interpreter;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace Parser;
public class Parser
{
    private static readonly char[] lineSeparators = { '\n' };
    private static readonly char commentSymbol = '#';
    private static readonly char directorySymbol = '@';

    private string[] linesText;
    private Apps.Interpreter interpreter;

    private Line[] parsedLines = null;
    public Parser(string text, Apps.Interpreter interpreter)
    {
        this.linesText = splitLines(text);
        this.interpreter = interpreter;
    }

    public Parser(string[] lines, Apps.Interpreter interpreter)
    {
        this.linesText = lines;
        this.interpreter = interpreter;
    }

    public Parser(FileManager.FileManager manager, Apps.Interpreter interpreter)
    {
        this.linesText = splitLines(manager.GetContent());
        this.interpreter = interpreter;
    }


    public Line[] Parse(bool forceParse = false)
    {
        if (forceParse || parsedLines == null)
        {
            parsedLines = parseLines();
        }

        List<Line> lines = new List<Line>();
        foreach (Line line in parsedLines)
        {
            if (!line.Whole.StartsWith(directorySymbol))
            {
                lines.Add(line);
            }
            else
            {
                lines.Add(new Line("", [], "", ""));
            }
        }
        return lines.ToArray();
    }


    private string[] splitLines(string text)
    {
        return text.Split(lineSeparators);
    } 

    private Line[] parseLines()
    {
        removeCommentsAndMultiSpaces();

        Line[] lines = new Line[linesText.Length];
        for (int i = 0; i < linesText.Length; i++)
        {
            lines[i] = parseLine(linesText[i]);
        }

        return lines;
    }

    private Line parseLine(string lineText)
    {
        string command = "";
        List<ulong> parameters = new List<ulong> { };
        string file = "";
        Regex regex = new Regex("\\s");
        string[] strings = Regex.Split(lineText, @"\s(?=(?:[^""]*""[^""]*"")*[^""]*$)");
        List<string> words = new List<string>();
        for (int i = 0; i < strings.Length; i++)
        {
            words.Add(strings[i]);
        }
        
        if (lineText.StartsWith(directorySymbol))
        {
            return new Line("", [], "", lineText);
        }

        for (int i = 0; i < words.Count; i++)
        {
            switch (i)
            {
                case 0: 
                    command = words[i];
                    break;
                default:
                    (List<ulong>, string) val = ParseText(words[i]);
                    parameters.AddRange(val.Item1);
                    file = val.Item2;
                    break;
            }
        }
        return new Line(command, parameters.ToArray(), file, lineText);
    }

    public static ulong CharToULong(char text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text.ToString());
        Array.Resize(ref bytes, 8);
        return BitConverter.ToUInt64(bytes, 0);
    }

    public static (List<ulong>, string) ParseText(string text)
    {
        (List<ulong>, string) parameters = (new List<ulong>(), "");

        text = text.Trim();

        try
        {
            if (string.IsNullOrEmpty(text))
            {
                return parameters;
            }
            if (text[0] == '"')
                throw new Exception();


            try
            {
                parameters.Item1.Add(ulong.Parse(text));
            }
            catch
            {
                if (text.ToUpper() == text)
                {
                    parameters.Item2 = text;
                }
                else
                {
                    throw;
                }
            }
            return parameters;
        }
        catch
        {
            string newText = "";
            if (!Regex.Match(text, "(\")").Success)
                newText = text;

            string value = Regex.Match(text, """^\s*"([^"]*)"\s*$""").Groups[1].Value;
            if (value != "")
            {
                newText = value;
            }
            for (int c = 0; c < newText.Length; c++)
            {
                parameters.Item1.Add(CharToULong(newText[c]));
            }

            parameters.Item1.Add(0);

            //throw new Exception("Invalid string - expected a number, received " + words[i]);
        }


        return parameters;
    }


    public (string, string)[] GetUsedDirectories()
    {
        List<(string, string)> usedFiles = new List<(string, string)>();
        Parse();
        for (int i = 0; i < parsedLines.Length; i++)
        {
            Line line = parsedLines[i];
            if (line.Whole.StartsWith(directorySymbol))
            {
                string curLine = line.Whole.Trim().Substring(1).Trim();
                string alias = curLine.Split(" as ")[1];
                string directory = curLine.Split(" as ")[0];
                directory += directory.EndsWith(".smpl") ? "" : ".smpl";
                try
                {
                    FileManager.FileManager manager = new FileManager.FileManager(directory, null);
                    manager.GetContent();
                    usedFiles.Add((directory, alias));
                }
                catch
                {
                    interpreter.Error(new ParserException($"Invalid directory: {directory}"));
                }
            }
        }

        return usedFiles.ToArray();
    }

    private void removeCommentsAndMultiSpaces()
    {
        List<string> newLines = new List<string>();
        for (int i = 0; i < linesText.Length; i++)
        {
            string text = Regex.Replace(linesText[i], commentSymbol + @"(.*?)" + commentSymbol, "");
            //text = Regex.Replace(text, @"^\s+", "");
            newLines.Add(text);
        }

        linesText = newLines.ToArray();
    }



    public class ParserException : App.AppException
    {
        public ParserException(string message)
        {
            this.isError = true;
            this.message = $"Parser exception: {message}";
        }
    }
}
