using Libs.SecLib.Shells.PowerShell;

namespace Libs.SecLib.Shells.PowerShell {
    public class Shell : ShellBase, IShell, IShellCommands {
        public Shell() {
            _name = "PowerShell";
            _description = "Powershell shell.";
            Commands = new Commands();
        }

        public override string RunCommand(string command) {
            try {
                string commandString = "";
                if (command.EndsWith(".ps1"))
                    commandString = $"-ExecutionPolicy Bypass -File \"{command}\"";
                else
                    commandString = $"-Command \"{command}\"";

                return LaunchProcess("powershell.exe", commandString);
            }
            catch (Exception) {
                throw;
            }
        }

    }
}
