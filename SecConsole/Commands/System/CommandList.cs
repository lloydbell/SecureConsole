namespace SecConsole.Commands.System
{
    public class CommandList
    {
        public CommandList()
        {
            
        }

        private Dictionary<string, CommandDetails> _commands = [];
        public void Add(string filename, string folder, string options = "")
        {
            _commands.TryAdd(filename, new CommandDetails(filename, folder, options));
        }
        public void UpdateObscureName(string key, string value)
        {
            if (_commands.ContainsKey(key))
                _commands[key].ObscureName = value;
        }
        public void SetExplicit(string key, bool value)
        {
            if (_commands.ContainsKey(key))
                _commands[key].Explicit = value;
        }

        public void Remove(string key)
        {
            if (_commands.ContainsKey(key))
                _commands.Remove(key);
        }

        public string GetObscuredName(string key)
        {
            if (_commands.ContainsKey(key))
                return _commands[key].ObscureName;
            return key;
        }
        public List<CommandDetails> GetAll()
        {
            return _commands.Values.ToList();
        }
        public List<string> GetCommands()
        {
            return _commands.Keys.ToList();
        }

        public int Count()
        {
            return _commands.Count();
        }
    }
}