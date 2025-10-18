using Libs;

namespace SecConsole.Settings
{
    public class Colors
    {
        public Colors(){}

    public LineColors Default {get; set;} = new LineColors(ConsoleColor.Green, ConsoleColor.Black);
    public LineColors Commands {get; set;} = new LineColors(ConsoleColor.DarkGreen, ConsoleColor.Black);
    public LineColors Updates {get; set;} = new LineColors(ConsoleColor.Green, ConsoleColor.Black);
    public LineColors Spacers {get; set;} = new LineColors(ConsoleColor.DarkCyan, ConsoleColor.DarkBlue);
    public LineColors Warnings {get; set;} = new LineColors(ConsoleColor.Yellow, ConsoleColor.Black);
    public LineColors Errors {get; set;} = new LineColors(ConsoleColor.DarkRed, ConsoleColor.Black);
    public LineColors Debugging {get; set;} = new LineColors(ConsoleColor.Blue, ConsoleColor.Black);        

    }
}