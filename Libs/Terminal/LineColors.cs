using System;

namespace Libs.Terminal
{
    public class LineColors
    {
        public LineColors() { }

        public LineColors(ConsoleColor text, ConsoleColor background)
        {
            TextColor = text;
            BackColor = background;
        }

        public ConsoleColor TextColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackColor { get; set; } = ConsoleColor.Black;

        public ConsoleColor GetTextColor() { return TextColor; }
        public ConsoleColor GetBackColor() { return BackColor; }
    }
}