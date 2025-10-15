using System;
using System.Diagnostics;

namespace Libs.Terminal {
    public class Colored
    {
        public Colored() { }
        public Colored(ConsoleColor text, ConsoleColor background)
        {
            TextColor = text;
            BackgroundColor = background;
        }

        private void ResetColors()
        {
            Console.ForegroundColor = TextColor;
            Console.BackgroundColor = BackgroundColor;
        }

        public ConsoleColor TextColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        public void WriteLine(string text = "")
        {
            ResetColors();
            Console.WriteLine(text.PadRight(Console.WindowWidth - 1));

        }

        public void WriteLine(string text, ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(text.PadRight(Console.WindowWidth - 1));
            ResetColors();
        }

        public void Write(string text)
        {
            ResetColors();
            Console.WriteLine(text);
        }
        public void Write(string text, ConsoleColor foreground)
        {
            Console.ForegroundColor = foreground;
            Console.WriteLine(text);
            ResetColors();
        }
        public void Write(string text, ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(text);
            ResetColors();
        }
        
        public void Write(string text, LineColors lineColors)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(text);
            ResetColors();
        }        
    }  
}
