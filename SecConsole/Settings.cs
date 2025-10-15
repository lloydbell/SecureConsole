namespace SecConsole
{
    public class Settings
    {
        public Settings(){}
    
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

        public string ToJson(){

            string results = "";
            results += "{";
            results += $"\'WindowWidth\': \'{WindowWidth}\'";
            results += ",";
            results += $"\'WindowHeight\': \'{WindowHeight}\'";       
            results += "}";
            return results;
        }
    }
}