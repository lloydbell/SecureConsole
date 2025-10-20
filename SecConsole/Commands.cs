namespace SecConsole
{
    public class Commands
    {
        public Commands()
        {
            
        }

        private Dictionary<string, string> _commands = [];
        public void Add(string key, string value = "")
        {
            _commands.TryAdd(key, value);
        }
        public void Update(string key, string value = "")
        {
            if (_commands.ContainsKey(key))
                _commands[key] = value;
        }

        public void Remove(string key)
        {
            if (_commands.ContainsKey(key))
                _commands.Remove(key);
        }

        public string Get(string key)
        {
            if (_commands.ContainsKey(key))
                return _commands[key];
            return key;
        }
        public List<string> GetList()
        {
            return new List<string>(_commands.Keys);
        }

        public int Count()
        {
            return _commands.Count();
        }
    }
}