namespace Libs.Shells {
    public interface IShell
    {
        // Properties
        string Name { get; }
        string Description { get; }
        bool SuppressOutput { get; set; }
        bool UseShellExecute { get; set; }
        IShellCommands Commands { get; set; }
        string RunCommand(string command);
        string RunCommand(string command, string arguments);
        string RunCommand(string command, string arguments, string workingDirectory);
    }
}
