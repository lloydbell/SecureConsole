namespace SecConsole.Commands.Internal
{
    public class CommandList
    {
        public CommandList()
        {

        }
        
        public string Marker { get; set; } = "!";

        private Dictionary<string, CommandDetails> _commands = [];

        public void Add(CommandType id, string command, string options, string description)
        {
            Add(new CommandDetails(id, command, options, description));
        }
        public void Add(CommandDetails command)
        {
            _commands.TryAdd(command.Id.ToString(), command);
        }
        public void Remove(string key)
        {
            if (_commands.ContainsKey(key))
                _commands.Remove(key);
        }

        public CommandDetails? GetFromCommand(string command)
        {
            CommandDetails? result = null;
            foreach(CommandDetails cmd in _commands.Values){
                if(cmd.Command == command){
                    result = cmd;
                    break;
                }
            }
            return result;
        }
        public List<CommandDetails> GetAll()
        {
            return _commands.Values.ToList();
        }

        public int Count()
        {
            return _commands.Count();
        }
    }
}
