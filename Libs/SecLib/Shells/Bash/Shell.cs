namespace Libs.SecLib.Shells.Bash {
    public class Shell : ShellBase, IShell {
        public Shell() {
            _name = "Bash";
            _description = "Bash shell.";
            Commands = new Commands();
        }
        public override string RunCommand(string command) {
            try {
                return LaunchProcess(command, "");
            }
            catch (Exception) {
                throw;
            }
        }
    }
}
