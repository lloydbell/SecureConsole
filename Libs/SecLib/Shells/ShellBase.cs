using System.Diagnostics;

namespace Libs.SecLib.Shells {
    public class ShellBase {
        protected string _name = "";
        protected string _description = "";
        public ShellBase() { }
        public string Name { get => _name; }
        public string Description { get => _description; }
        public IShellCommands? Commands { get; set; }
        public bool SuppressOutput { get; set; } = false;
        public string CreateFile(string fullpath) {
            if (Commands == null)
                throw new MissingMemberException(nameof(Commands));
            return RunCommand(Commands.CreateFile(fullpath));
        }
        public string WriteFile(string fullpath, string contents) {
            if (Commands == null)
                throw new MissingMemberException(nameof(Commands));
            return RunCommand(Commands.WriteFile(fullpath, contents));
        }
        public string OwnFile(string fullpath, string username) {
            if (Commands == null)
                throw new MissingMemberException(nameof(Commands));
            return RunCommand(Commands.OwnFile(fullpath, username));
        }
        public virtual string RunCommand(string command) {
            return LaunchProcess(command, "");
        }
        protected string LaunchProcess(string command, string arguments) {
            try {
                Process process = new();
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = $" {arguments}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardInput = true;

                if (!SuppressOutput) {
                    process.OutputDataReceived += (sender, args) => { Console.WriteLine($"OutputDataReceived: {args.Data}"); };
                    process.ErrorDataReceived += (sender, args) => { Console.WriteLine($"ErrorDataReceived: {args.Data}"); };
                }
                process.Start();
                if (process == null)
                    return $"Failed to run command {command}";
                // Wait for the process to exit
                while (!process.HasExited) {
                    global::System.Threading.Thread.Sleep(25);
                }
                // read the output from the command
                string output = process.StandardOutput.ReadToEnd();
                // Clean up
                process.Dispose();
                return output;
            }
            catch (Exception exception) {
                return exception.Message;
            }
        }
    }
}
