namespace SecConsole
{
    public static class Commands
    {
        static Commands()
        {
            Add("apt");
            Add("cat");
            Add("cd");
            Add("curl");
            Add("chfn");
            Add("chmod");
            Add("chown");
            Add("clear");
            Add("chsh");
            Add("ciptool");
            Add("cmake");
            Add("echo");
            Add("history");
            Add("id");
            //Add("find"); 
            Add("netstat");
            Add("last");
            Add("open");
            Add("passwd");
            Add("ping");
            Add("ps");
            Add("scp");
            Add("su");
            Add("sudo", "lloyd");
            Add("sestatus");
            Add("ssh");
            Add("ssh-keygen");
            Add("ls");
            Add("top");
            Add("touch");
            Add("type");
            Add("ufw");
            Add("visudo");
            Add("wget");
            Add("whoami");
        }

        private static Dictionary<string, string> _commands = [];
        public static void Add(string key, string value = "")
        {
            _commands.TryAdd(key, value);
        }
        public static void Update(string key, string value = "")
        {
            if (_commands.ContainsKey(key))
                _commands[key] = value;
        }

        public static void Remove(string key)
        {
            if (_commands.ContainsKey(key))
                _commands.Remove(key);
        }

        public static string Get(string key)
        {
            if (_commands.ContainsKey(key))
                return _commands[key];
            return key;
        }
        public static List<string> GetList()
        {
            return new List<string>(_commands.Keys);
        }

        public static int Count()
        {
            return _commands.Count();
        }
    }
}