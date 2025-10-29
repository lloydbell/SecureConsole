namespace SecConsole.Commands.System
{
    public class CommandDetails
    {
        public CommandDetails(string originalName, string folder, string options = "")
        {
            OriginalName = originalName;          
            ObscureName = originalName;  
            Folder = folder;
            Options = options;
        }
        public bool Explicit { get; set; } = false;
        public string OriginalName { get; set; } = "";
        public string ObscureName { get; set; } = "";
        public string Folder { get; set; } = "";
        public string Options { get; set; } = "";
        public string OriginalPath { 
            get{
                return $"{Folder}{OriginalName}";
            }
        }    
        public string ObscurePath { 
            get{
                return $"{Folder}{ObscureName}";
            }
        }
        public bool Obscured { 
            get{
                return OriginalName == ObscureName;
            }
        }        
    }
}