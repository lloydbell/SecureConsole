namespace SecConsole;

public class CommandBuffer
{  
    private List<string> _commands = [];
    public CommandBuffer(){}

    public int Position {get; set;} = 0;
    public int Limit {get; set;} = 25;
       
        public void Add(string command) {
            _commands.Add(command);
            while(_commands.Count() > Limit)
                _commands.RemoveAt(0);
            Position = _commands.Count() - 1;
        }

    public string Next(){
        if(_commands.Count() == 0)
            return "";
        Position++;
        if(Position >= _commands.Count())
            Position = 0;
        return _commands[Position];
    } 

    public string Previous() {
        if(_commands.Count() == 0)
            return "";        
        Position--;
        if(Position < 0)
            Position = _commands.Count() - 1;
        return _commands[Position];
    }  
    public string Last() {
        if(_commands.Count() == 0)
            return "";        
        Position = 0;
        return _commands.Last();
    }         

}