namespace SecConsole
{
    public class SpecialCommands
    {
        public SpecialCommands()
        {

        }
        
        public string Marker { get; set; } = "!";

        private Dictionary<string, SpecialCommand> _commands = [];

        public void Add(CommandType id, string command, string options, string description)
        {
            Add(new SpecialCommand(id, command, options, description));
        }
        public void Add(SpecialCommand command)
        {
            _commands.TryAdd(command.Id.ToString(), command);
        }
        public void Remove(string key)
        {
            if (_commands.ContainsKey(key))
                _commands.Remove(key);
        }

        public SpecialCommand? GetFromCommand(string command)
        {
            SpecialCommand? result = null;
            foreach(SpecialCommand cmd in _commands.Values){
                if(cmd.Command == command){
                    result = cmd;
                    break;
                }
            }
            return result;
        }
        public List<SpecialCommand> GetAll()
        {
            return _commands.Values.ToList();
        }

        public int Count()
        {
            return _commands.Count();
        }
    }
}
