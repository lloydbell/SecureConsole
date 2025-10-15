namespace SecConsole;

public class LocalizedStringsBase : ILocalizedStrings
{  
    private Dictionary<string, string> _strings = [];
    public LocalizedStringsBase(){}

    public string Language { get => ""; }

        
        public void Add(string key, string value) {
            _strings.TryAdd(key, value);
        }

    public string Get(string key)
    {
          if (_strings.ContainsKey(key))
                return _strings[key];
          return "";
    } 

}
