namespace Libs.Shells.Bash {
    public class Commands : IShellCommands {
        public Commands() { }

        public string CreateFile(string fullpath) {
            return $"sudo touch {fullpath}";
        }
        public string WriteFile(string fullpath, string contents) {
            return $"sudo echo {contents} >| {fullpath}";
        }
        public string OwnFile(string fullpath, string username) {
            return $"sudo chown {username} {fullpath}";
        }
    }
}
