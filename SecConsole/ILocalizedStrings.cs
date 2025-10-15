namespace SecConsole{
    public interface ILocalizedStrings
    {
        string Language { get; }
        void Add(string key, string value);
        string Get(string key);
    }
}
