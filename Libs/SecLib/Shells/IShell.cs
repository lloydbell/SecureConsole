namespace Libs.SecLib.Shells {
    public interface IShell {
        // Properties
        string Name { get; }
        string Description { get; }
        bool SuppressOutput { get; set; }
        IShellCommands Commands { get; set; }
    }
}
