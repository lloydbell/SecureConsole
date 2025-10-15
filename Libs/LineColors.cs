namespace Libs;

    public class LineColors
    {
        public LineColors() { }

        public LineColors(ConsoleColor text, ConsoleColor background)
        {
            TextColor = text;
            BackgroundColor = background;
        }

        public ConsoleColor TextColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        public ConsoleColor GetTextColor() { return TextColor; }
        public ConsoleColor GetBackColor() { return BackgroundColor; }
    }
