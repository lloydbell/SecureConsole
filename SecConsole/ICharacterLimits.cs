namespace SecConsole{
    public interface ICharacterLimits
    {
        string Language { get; }
        void Add(string key, int value);
        int Get(string key);
    }
}
