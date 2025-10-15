namespace Libs.Shells.Bash {
    public class Shell : ShellBase, IShell
    {
        public Shell()
        {
            _name = "Bash";
            _description = "Bash shell.";
            Commands = new Commands();
        }
        public override string RunCommand(string command)
        {
            try
            {
                return LaunchProcess(command, "");
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public override string RunCommand(string command, string arguments) {
            try
            {
                return LaunchProcess(command, arguments);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override string RunCommand(string command, string arguments, string workingDirectory) {
            try
            {
                return LaunchProcess(command, arguments, workingDirectory);
            }
            catch (Exception)
            {
                throw;
            }
        }        

    }
}
