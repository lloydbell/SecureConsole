using Libs;

namespace SecConsole.Settings
{
    public class Terminal
    {
        public Terminal() { }
        public string PromptMarker { get; set; } = "$ ";

        public int WindowWidth
        {
            get
            {
                return Console.WindowWidth;
            }
            set
            {
                Console.WindowWidth = value;
            }
        }
        public int WindowHeight {
                        get{
                return Console.WindowHeight;
            }
            set{
                Console.WindowHeight = value;
            }
        } 
     }
}