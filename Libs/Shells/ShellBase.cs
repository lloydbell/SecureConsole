using System.Diagnostics;

namespace Libs.Shells {
    public class ShellBase {
        protected string _name = "";
        protected string _description = "";
        public ShellBase() { }
        public string Name { get => _name; }
        public string Description { get => _description; }
        public IShellCommands? Commands { get; set; }
        public bool UseShellExecute { get; set; } = false;
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
        public virtual string RunCommand(string command, string arguments) {
            return LaunchProcess(command, arguments);
        }  
        public virtual string RunCommand(string command, string arguments, string workingDirectory) {
            return LaunchProcess(command, arguments, workingDirectory);
        }   



        protected string LaunchProcess(string command, string arguments, string workingDirectory = "/")
        {
            try
            {
                Process process = new();
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = UseShellExecute;
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardInput = true;

                if (!SuppressOutput)
                {
                    process.OutputDataReceived += (sender, args) => { Console.WriteLine($"OutputDataReceived: {args.Data}"); };
                    process.ErrorDataReceived += (sender, args) => { Console.WriteLine($"ErrorDataReceived: {args.Data}"); };
                }
                process.Start();
                if (process == null)
                    return $"Failed to run command {command}";
                // Wait for the process to exit
                while (!process.HasExited)
                {
                    global::System.Threading.Thread.Sleep(25);
                }
                // read the output from the command
                string output = process.StandardOutput.ReadToEnd();
                output = output.TrimEnd();
                // Clean up
                process.Dispose();
                return output;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }
    }
}
