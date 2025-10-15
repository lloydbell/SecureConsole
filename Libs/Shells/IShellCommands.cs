namespace Libs.Shells {
    public interface IShellCommands {
        string CreateFile(string fullpath);
        string WriteFile(string fullpath, string contents);
        string OwnFile(string fullpath, string username);
    }
}
