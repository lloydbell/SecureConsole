namespace Libs;

public class ColorTerminal
{
    private int _inputWait = 100;
    private string _input = "";
    private string _blankLine = "";
    public ColorTerminal() { 
        CalculateBlankLine();
    }
    public ColorTerminal(ConsoleColor textColor, ConsoleColor backgroundColor)
    {
        TextColor = textColor;
        BackgroundColor = backgroundColor;
        CalculateBlankLine();
    }

    public ColorTerminal(LineColors lineColors)
    {
        TextColor = lineColors.TextColor;
        BackgroundColor = lineColors.BackgroundColor;
        CalculateBlankLine();
    }

    public string TerminalString {get; set;} = "$ ";

    private void ResetColors()
    {
        Console.ForegroundColor = TextColor;
        Console.BackgroundColor = BackgroundColor;
    }

    public ConsoleColor TextColor { get; set; } = ConsoleColor.White;
    public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
    public string Input {
        get{return _input;} 
        set{
            _input = value;
            ClearLine();
            string s = $"{TerminalString}{_input}";
            Console.Write(s);           
        }
    }

    private void CalculateBlankLine(){
        _blankLine = CalculateFilledLine(" ");  
    }

    private string CalculateFilledLine(string character){
        int max = Console.BufferWidth;
        string s = "";
        while (s.Length < max)
            s += character;
        return s;   
    }

    public void ClearLine()
    {
        Console.CursorLeft = 0;
        Console.Write(_blankLine);
        Console.CursorLeft = 0;
    }

    public void WriteDevider(string text, LineColors lineColors)
    {
        if (text.Length == 0)
            text = " ";
        Console.ForegroundColor = lineColors.TextColor;
        Console.BackgroundColor = lineColors.BackgroundColor;
        string tmp = CalculateFilledLine(text);
        Console.WriteLine(tmp);
        ResetColors();
    }

    public void WriteLine(string text)
    {
        if (text.Length == 0)
            return;
        ResetColors();
        int max = Console.WindowWidth - text.Length;
        string s = "";
        while (s.Length < max)
            s += text;
        Console.WriteLine(s);
        ResetColors();
    }

    public void WriteLine(string text, LineColors lineColors)
    {
        if (text.Length == 0)
            return;
        Console.ForegroundColor = lineColors.TextColor;
        Console.BackgroundColor = lineColors.BackgroundColor;
        string tmp = _blankLine;
        tmp = tmp.Substring(text.Length);
        string s = $"{text}{tmp}";
        Console.WriteLine(s);
        ResetColors();
    }

    public string Prompt(string text = "")
    {
        ResetColors();
        Input = text;
        //Console.Write(text);       
        bool loop = true;
        bool flash = true;
        while (loop)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Enter){
                loop = false;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace){
                string s = Input;
                if (s.Count() > 0){
                    s = s.Substring(0, s.Count() - 1);
                    Input = s;
                }
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow){
                return Constants.RIGHTARROW;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow){
                return Constants.LEFTARROW;
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow){
                return Constants.UPARROW;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow){
                return Constants.DOWNARROW;
            }
            else {
                Input += keyInfo.KeyChar;
            }
            System.Threading.Thread.Sleep(_inputWait);
        }
        ResetColors();
        return Input;
    }

    public void Write(string text)
    {
        ResetColors();
        Console.Write(text);
        ResetColors();
    }
    public void Write(string text, ConsoleColor foreground)
    {
        ResetColors();
        Console.ForegroundColor = foreground;
        Console.Write(text);
        ResetColors();
    }
    public void Write(string text, ConsoleColor foreground, ConsoleColor background)
    {
        ResetColors();
        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
        Console.Write(text);
        ResetColors();
    }

    public void Write(string text, LineColors lineColors)
    {
        ResetColors();
        Console.ForegroundColor = lineColors.TextColor;
        Console.BackgroundColor = lineColors.BackgroundColor;
        Console.Write(text);
        ResetColors();
    }

    public void OverWrite(string text, LineColors lineColors)
    {
        Console.ForegroundColor = lineColors.TextColor;
        Console.BackgroundColor = lineColors.BackgroundColor;
        
        Console.CursorTop = Console.CursorTop - 1;
        int max = Console.BufferWidth;
        ClearLine();
        Console.WriteLine(text);
        ResetColors();
    }    
        
    public void WriteCentered(string text)
    {
        ResetColors();
        int max = Console.WindowWidth;
        int n = (max - text.Length) / 2;
        string s = "";
        for (int i = 0; i < n; i++)
            s += " ";
        s += text;
        while (s.Length < max)
            s += " ";       
        Console.WriteLine(s);
    }      


    public void WriteInColumns(string[] values){
        ResetColors();
        int max = Console.WindowWidth;
        int longestString = values.Select(x => x.Count()).Max();
        int columns = (int)Math.Floor((double)max/(double)longestString);
        int colWidth = (int)Math.Floor((double)max/(double)columns);
        max = colWidth * columns;
        int x = 0;
        string line = "";
        foreach(string s in values)
        {
            Console.CursorLeft = x;
            Console.Write(s);
            x += colWidth;
            if(x >= max){
                Console.WriteLine();
                x = 0;
            }
        }
        Console.WriteLine();
           
    }      

}
