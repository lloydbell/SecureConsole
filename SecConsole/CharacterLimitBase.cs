namespace SecConsole;

public class CharacterLimitBase : ICharacterLimits
{  
    private Dictionary<string, int> _values = [];
    public CharacterLimitBase(){}

    public string Language { get => ""; }

        
        public void Add(string key, int value) {
            _values.TryAdd(key, value);
        }

    public int Get(string key)
    {
          if (_values.ContainsKey(key))
                return Convert.ToInt32(_values[key]);
          return -1;
    } 

}
