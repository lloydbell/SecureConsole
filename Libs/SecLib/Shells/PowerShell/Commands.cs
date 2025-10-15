namespace Libs.SecLib.Shells.PowerShell {
    public class Commands : IShellCommands {
        public Commands() { }

        public string CreateFile(string fullpath) {
            return $"NOT IMPLEMENTED";
        }
        public string WriteFile(string fullpath, string contents) {
            return $"NOT IMPLEMENTED";
        }
        public string OwnFile(string fullpath, string username) {
            return $"NOT IMPLEMENTED";
        }
    }
}
