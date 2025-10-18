using Libs;

namespace SecConsole
{
    public class Settings
    {
        public Settings(){}

    public ColorSettings Colors {get; set;} = new ColorSettings();       

        public string PromptMarker{get; set;} = "$ ";

        public int WindowWidth {
            get{
                return Console.WindowWidth;
            }
            set{
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