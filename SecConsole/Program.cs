using System;
using System.Text;
using System.IO;
using System.Formats.Asn1;
using System.Text.Json;
using System.Security.Cryptography;
using Libs;
using Libs.SecLib;
using Libs.SecLib.Encryption;

namespace SecConsole;

class Program
{
    static private int _exitCode = 0;
    static private string _key = "thisisareallylongkey";
    static private string _results = "";
    static private string _workingDirectory = "";
    static private string _fileDirectory = "/usr/bin/";
    static private string _settingsFile = "settings.json";
    static private Commands _commands = new Commands();
    static private SpecialCommands _specialCommands = new SpecialCommands();
    static private Settings.Application _settings = new();
    static private Bash bash = new Bash();
    static private Libs.ColorTerminal Terminal = new ColorTerminal();
    static private Scramble _scramble = new Scramble("");
    static private Libs.Shells.Bash.Shell _shell = new();
    static private CommandBuffer _buffer = new CommandBuffer();
    static private int _defaultDelay = 25;
    static private bool _isFirstRun = true;
    static private bool _debugging = true;

    public static void Main(string[] args)
    {
        _specialCommands.Add(CommandType.MENU, "menu", "", "Displays the menu.");
        _specialCommands.Add(CommandType.BLUR, "blur", "", "Obsificates the shell commands.");
        _specialCommands.Add(CommandType.FOCUS, "focus", "", "Restores the shell commands.");
        _specialCommands.Add(CommandType.RUN, "run", "<filepath>", "Runs a shell script file.");
        _specialCommands.Add(CommandType.CONVERT, "convert", "<filepath>", "Converts a shell script file to use obsificated commands.");
        _specialCommands.Add(CommandType.ENCRYPT, "encrypt", "<filepath>", "Encrypts a file.");
        _specialCommands.Add(CommandType.DECRYPT, "decrypt", "<filepath>", "Decrypts a file.");
        _specialCommands.Add(CommandType.SHOW, "show", "<command>", "Displays the current value for a shell command.");
        _specialCommands.Add(CommandType.PAST, "past", "", "Show terminal history.");
        _specialCommands.Add(CommandType.TEST, "test", "", "Test Obsificated names.");
        _specialCommands.Add(CommandType.QUIT, "quit", "", "Quits the application");

        _commands.Add("apt");
        _commands.Add("cat");
        _commands.Add("cd");
        _commands.Add("curl");
        _commands.Add("chfn");
        _commands.Add("chmod");
        _commands.Add("chown");
        _commands.Add("clear");
        _commands.Add("chsh");
        _commands.Add("ciptool");
        _commands.Add("cmake");
        _commands.Add("echo");
        _commands.Add("history");
        _commands.Add("id");
        //_commands.Add("find"); 
        _commands.Add("netstat");
        _commands.Add("last");
        _commands.Add("open");
        _commands.Add("passwd");
        _commands.Add("ping");
        _commands.Add("ps");
        _commands.Add("scp");
        _commands.Add("su");
        _commands.Add("sudo", "lloyd");
        _commands.Add("sestatus");
        _commands.Add("ssh");
        _commands.Add("ssh-keygen");
        _commands.Add("ls");
        _commands.Add("top");
        _commands.Add("touch");
        _commands.Add("type");
        _commands.Add("ufw");
        _commands.Add("visudo");
        _commands.Add("wget");
        _commands.Add("whoami");

        LoadSettings();

        //Console.SetWindowSize(_settings.Terminal.WindowWidth, _settings.Terminal.WindowHeight);        
        //Console.SetWindowSize(_settings.Terminal.WindowWidth, _settings.Terminal.WindowHeight);
        Terminal = new(_settings.Colors.Default);
        EnglishStrings _strings = new EnglishStrings();
        EnglishCharacterLimits _limits = new EnglishCharacterLimits();
        
        bash = new Bash();
        IsFirstRun();
        System.Threading.Thread.Sleep(_defaultDelay);
        
        //ClearTerminal();

        ShowDevider(" ");
        Terminal.WriteCentered(_strings.Get("ApplicationName"));
        Terminal.WriteCentered(_strings.Get("Copyright"));
        Terminal.WriteCentered(_strings.Get("ApplicationDescription"));
        ShowDevider(" ");

        if (_isFirstRun)
        {
            DisplaySystemInfo();
        }
        ShowDevider(" ");
        ShowMessage(" ");

        
        /*
        if (_isFirstRun)
            Terminal.Write(_strings.Get("DescriptionName"), _warningColors);
        tmp = "";    
        while(tmp == "")
            tmp = ShowPrompt(_strings.Get("PromptName"), _strings.Get("StringName"), _limits.Get("NameMin"), _limits.Get("NameMax"), ['1','2','3','4','5','6','7','8','9','0','!','@','#','$','%','^','&','*','(',')','-','=','-','+']);                      
        _commands.Update("sudo", tmp);
        */

        if (_key == "")
        {
            if (_isFirstRun)
                ShowUpdate(_strings.Get("DescriptionKey"));
            _key = GetKey(_strings, _limits);
        }

        _scramble = new(_key);

        if (_isFirstRun)
        {
            ShowUpdate(_strings.Get("FirstRunMessage"));
            ObsificateFileNames();
        }
        else
        {
            bool passed = TestKey(_key);
            while (!passed)
            {
                ShowWarning(_strings.Get("KeyInvalid"));
                _key = GetKey(_strings, _limits);
                passed = TestKey(_key);
            }

            ShowUpdate(_strings.Get("KeyValid"));
        }

        ShowWorkingDirectory();
        
        while (_exitCode == 0)
        {
            _results = Terminal.Prompt();
            // check for arrow keys
            if (_results == Constants.UPARROW)
            {
                Terminal.Prompt(_buffer.Previous());
                continue;
            }
            else if (_results == Constants.DOWNARROW)
            {                
                Terminal.Prompt(_buffer.Next());
                continue;
            }

            _results = BufferCommand(_results);
            // handle special commands
            if(_results.StartsWith(_specialCommands.Marker)){
                HandleSpecialCommand(_results);
            }                      
            else
            {
                bool results = ExecuteHandledCommands(_results);
                if (results == false)
                {
                    ExecuteCommand(_results);
                }
            }

        }
    }

    private static void HandleSpecialCommand(string command){
        string option = "";
        command = command.Replace(_specialCommands.Marker, "");
        if(command.Contains(" "))
        {
            string[] parts = command.Split(" ");
            command = parts[0];
            option = parts[1];
        }
        SpecialCommand? commandDetails = _specialCommands.GetFromCommand(command);
        if(commandDetails == null){
            ShowError("Command not found");
            return;
        }

            if(commandDetails.Id == CommandType.MENU)
            {
                DisplayMenu();
            }
            else if(commandDetails.Id == CommandType.BLUR)
            {
                ObsificateFileNames();
            }
            else if(commandDetails.Id == CommandType.FOCUS)
            {
                ResetFileNames();
            } 
            else if(commandDetails.Id == CommandType.RUN)
            {
                // TODO: Run shell script
            } 
            else if(commandDetails.Id == CommandType.CONVERT)
            {
                // TODO: Convert shell script
            }                         
            else if(commandDetails.Id == CommandType.ENCRYPT)
            {
                EncryptFile(option);
            } 
            else if(commandDetails.Id == CommandType.DECRYPT)
            {
                DecryptFile(option);
            } 
            else if(commandDetails.Id == CommandType.SHOW)
            {
                ShowCommandValue(option);
            } 
            else if(commandDetails.Id == CommandType.TEST)
            {
                TestKey(_key);
            }  
            else if(commandDetails.Id == CommandType.PAST)
            {
                DisplayCommandHistory();
            }                         
            else if(commandDetails.Id == CommandType.QUIT)
            {
                ResetFileNames();
                _exitCode = 1;
            } 

    }

    private static string GetKey(ILocalizedStrings strings, ICharacterLimits limits)
    {
        string tmp = "";
        while (tmp == "")
            tmp = ShowPrompt(strings.Get("PromptKey"), strings.Get("StringKey"), limits.Get("KeyMin"), limits.Get("KeyMax"), ['!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=', '-', '+']);
        System.Threading.Thread.Sleep(500);
        return tmp;
    }

    private static void DisplaySystemInfo()
    {
        LineColors firstColor = new LineColors(ConsoleColor.Blue, ConsoleColor.Black);
        LineColors secondColor = new LineColors(ConsoleColor.White, ConsoleColor.Black); ;
        int count = 1;
        string seperator = ": ";

        AddMessage($"    ", secondColor);
        AddMessage($"Kernal{seperator}", firstColor);
        AddEndMessage($"{_shell.RunCommand("uname", "-s")}", secondColor);

        AddMessage($"    ", secondColor);
        AddMessage($"Hostname{seperator}", firstColor);
        AddEndMessage($"{_shell.RunCommand("uname", "-n")}", secondColor);

        AddMessage($"    ", secondColor);
        AddMessage($"CPU{seperator}", firstColor);
        AddEndMessage($"{_shell.RunCommand("uname", "-p")}", secondColor);

        AddMessage($"    ", secondColor);
        AddMessage($"OS{seperator}", firstColor);
        AddEndMessage($"{_shell.RunCommand("uname", "-o")}", secondColor);                

    }

    private static void DisplayMenu()
    {
        ClearTerminal();
        LineColors firstColor = _settings.Colors.Default;
        LineColors secondColor = _settings.Colors.Commands;
        LineColors deviderColor = new LineColors(_settings.Colors.Commands.TextColor, _settings.Colors.Commands.TextColor);
        string marker = _specialCommands.Marker;
        ShowDevider(" ", deviderColor);
        Terminal.WriteCentered("Menu");
        ShowDevider(" ", deviderColor);

        foreach(SpecialCommand cmd in _specialCommands.GetAll())
        {
            AddMessage($"    ", firstColor);
            AddMessage($"{marker}{cmd.Command} {cmd.Options} ", firstColor);
            AddEndMessage($"{cmd.Description}", secondColor);
        }     

        ShowDevider(" ", deviderColor);
        ShowMessage(" ");
    }

    private static void DisplayCommandHistory()
    {
        List<string> pastCommands = _buffer.GetAll();
        for(int i = 0; i < pastCommands.Count(); i++)
        {
            ShowMessage($"{i}. {pastCommands[i]}");
        }

    }

    private static string GetWorkingDirectory()
    {
        if (String.IsNullOrWhiteSpace(_workingDirectory))
        {
            BashResult results = bash.Command("pwd");
            if (results.ExitCode == 0)
                _workingDirectory = results.Output;
        }
        return _workingDirectory;
    }

    private static void UpdateWorkingDirectory(string target)
    {
        if(target.Contains('/') == false){
            target = $"{_workingDirectory}/{target}";
        }
        string fullCommand = $"{_commands.Get("cd")} '{target}'";
        BashResult results = bash.Command(fullCommand);
        if (results.ExitCode != 0){
            ShowError("Invalid path");
            return;
        }
        _workingDirectory = target;    
    }

    private static void ShowWorkingDirectory()
    {
        Console.Title = $"{GetWorkingDirectory()}";
    }

    private static bool ExecuteHandledCommands(string fullCommand)
    {
        //Terminal.Write($"ExecuteHandledCommands( {fullCommand} )", _debugColors);  
        if (fullCommand.StartsWith("cd "))
        {
            string[] parts = fullCommand.Split(" ");
            if (parts[1] == "..")
            {
                string path = GetWorkingDirectory();
                string shorterPath = path.Substring(0, path.LastIndexOf("/"));
                _workingDirectory = shorterPath;
            }
            else
            {
                //_workingDirectory = parts[1];
                UpdateWorkingDirectory(parts[1]);

            }
            ShowWorkingDirectory();
            Terminal.ClearLine();
            return true;
        }
        if (fullCommand == "ls")
        {
            string command = $"{_commands.Get("ls")} -a '{GetWorkingDirectory()}'";
            BashResult results = bash.Command(command);
            if (results.ExitCode == 0)
            {
                Terminal.WriteInColumns(results.Lines);
            }
            else
            {
                ShowError(results.ErrorMsg);
            }
            return true;
        }      
        else if (fullCommand.StartsWith("ls "))
        {
            string command = $"{_commands.Get("ls")}";
            fullCommand = fullCommand.Replace("ls ", $"{command} -a '{GetWorkingDirectory()}' ");
            BashResult results = bash.Command(fullCommand);
            ShowDebug($"fullCommand = {fullCommand}   results.ExitCode:{results.ExitCode}");
            if (results.ExitCode == 0)
            {
                Terminal.WriteInColumns(results.Lines);
            }
            else
            {
                ShowError(results.ErrorMsg);
            }
            return true;
        }
        else if (fullCommand == "clear")
        {
            ClearTerminal();
            return true;
        } 
        else if (fullCommand == "swd")
        {
            ShowWorkingDirectory();
            return true;
        }
        return false;
    }

    private static bool ExecuteCommand(string fullCommand)
    {
        if (fullCommand.Contains(" "))
            return ExecuteComplexCommand(fullCommand);
        else
            return ExecuteSimpleCommand(fullCommand);

    }

    private static bool ExecuteSimpleCommand(string fullCommand)
    {
        string tmp = _commands.Get(fullCommand);
        string command = String.IsNullOrWhiteSpace(tmp) ? fullCommand : tmp;
        ShowDebug($"Command - {fullCommand} --> {command}");
        var bashResults = bash.Command(command);
        foreach (var line in bashResults.Lines)
        {
            ShowMessage(line);
        }
        return bashResults.ExitCode == 0;

    }

    private static bool ExecuteComplexCommand(string fullCommand)
    {

        string[] parts = _results.Split(" ");
        for (int i = 0; i < parts.Length; i++)
        {
            string tmpPart = _commands.Get(parts[i]);
            if (!String.IsNullOrWhiteSpace(tmpPart))
                parts[i] = tmpPart;
        }
        string updatedCommand = String.Join(" ", parts);
        ShowDebug($"Command -> {updatedCommand}");

        BashResult results = bash.Command(updatedCommand);
        if (results.ExitCode == 0)
        {
            foreach (var line in results.Lines)
            {
                ShowMessage(line);
            }
        }
        return true;
    }

    private static bool IsFirstRun()
    {
        string location = "sudo";
        _isFirstRun = FindFile(location);
        return _isFirstRun;
    }

    private static bool TestKey(string key)
    {
        var list = _commands.GetList();
        int total = _commands.Count();
        int count = 0;
        string findCommand = _commands.Get("find");
        foreach (string command in list)
        {
            string obfuscatedCommand = _commands.Get(command);
            bool success = FindFile(obfuscatedCommand);
            if (success)
                count++;
        }
        ShowMessage($"{count}/{total} matches found!");
        return count > 0;
    }

    private static bool FindFile(string fileName)
    {
        //string findCommand = _commands.Get("find");
        string findCommand = "find";
        var results = bash.Command($"{_fileDirectory}{findCommand} {_fileDirectory}{fileName}");
        System.Threading.Thread.Sleep(_defaultDelay);
        return results.Output.Length > 0;
    }

    private static void ObsificateCommandNames()
    {
        foreach (string command in _commands.GetList())
        {
            string obsfucated = _commands.Get(command);
            if (obsfucated == "")
            {
                obsfucated = _scramble.HashString(command);
                _commands.Update(command, obsfucated);
            }
        }
    }

    private static void ObsificateFileNames()
    {
        ObsificateCommandNames();
        ShowMessage($"Converting {_commands.Count()} system commands.");
        foreach (string command in _commands.GetList())
        {
            bool originalFound = FindFile(command);
            string obsfucated = _commands.Get(command);
            if (originalFound)
            {                
                bash.Mv($"{_fileDirectory}{command}", $"{_fileDirectory}{obsfucated}");
                // Test for success
                bool success = FindFile(obsfucated);
                string successString = success ? "Done" : "Failed";
                LineColors useColors = success ? _settings.Colors.Updates : _settings.Colors.Warnings;
                ShowUpdate($"{successString} Converting: {command} -> {obsfucated}", useColors);
                if (!success)
                {
                    _commands.Update(command, command);
                }
            }
            else
            {
                bool obsfucatedFound = FindFile(obsfucated);
                if (!obsfucatedFound)
                {
                    ShowUpdate($"{command} Not Found!");
                    _commands.Remove(command);
                }
                else
                {
                    ShowUpdate($"{command} Found!");
                }
            }
        }
        ShowUpdate($"Convertion Complete!");

    }

    private static void ResetFileNames()
    {
        ShowMessage($"Reverting {_commands.Count()} system commands.");
        foreach (string command in _commands.GetList())
        {
            string currentCommand = _commands.Get(command);
            bash.Mv($"{_fileDirectory}{currentCommand}", $"{_fileDirectory}{command}");
            _commands.Update(command, command);
            ShowUpdate($"Reverting: {currentCommand} -> {command}");
            System.Threading.Thread.Sleep(_defaultDelay);
        }
        ShowMessage($"Revertion Complete!");
    }

    private static string ShowPrompt(string prompt, string value, int minChars, int maxChars, char[] omit)
    {
        string input = Terminal.Prompt(prompt);
        if (string.IsNullOrWhiteSpace(input))
        {
            ShowError($"{value} can not be blank.");
            return "";
        }
        if (input != null && input.Length < minChars)
        {
            ShowError($"{value} should be at least {minChars} characters.");
            return "";
        }
        if (input != null && input.Length > maxChars)
        {
            ShowError($"{value} should be no more then {maxChars} characters.");
            return "";
        }

        for (int i = 0; i < omit.Count(); i++)
        {
            string s = omit[i].ToString();
            if (input.Contains(s))
            {
                ShowError($"{value} can not contain the following character {s}.");
                return "";
            }
        }
        return input;
    }

    private static bool SaveSettings()
    {
        try{
            ShowDebug("SaveSettings()");
            string jsonString = JsonSerializer.Serialize(_settings);
            bool success = SaveFile(_settingsFile, jsonString);
            return true;
        }
        catch(Exception ex)
        {
            ShowError(ex.Message);
            return false;
        }        
    }

    private static void LoadSettings()
    {
        try{
            string jsonString = LoadFile(_settingsFile);
            Settings.Application? settings = JsonSerializer.Deserialize<Settings.Application>(jsonString);
            if(settings == null)
                _settings = new Settings.Application();
            _settings = settings;
        }
        catch(Exception ex)
        {
            ShowError($"Failed to load file. {ex.Message}");
            _settings = new Settings.Application();
            SaveSettings();
        }
    }

    private static string LoadFile(string filepath)
    {
        try
        {
            string content = File.ReadAllText(filepath);
            return content;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to load file {filepath} - {ex.Message}");
        }
        
    }

    private static bool SaveFile(string filepath, string content)
    {
        try
        {
            File.WriteAllText(filepath, content);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to save file {filepath} - {ex.Message}");
        }
        return true;
    }    

    private static string BufferCommand(string command)
    {
        command = command.TrimEnd();
        command = command.TrimEnd(Environment.NewLine.ToCharArray());
        _buffer.Add(command);
        ShowUpdate($"{Terminal.TerminalString}{_buffer.Last()}", _settings.Colors.Commands);
        return command;
    }

    private static void EncryptFile(string filepath){
        try{            
            string content = LoadFile(filepath);
            string encrypted = Libs.SecLib.Encryption.PlainText.Encrypt(content, _key);
            SaveFile(filepath, encrypted);
        }
        catch(Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private static void DecryptFile(string filepath){
        try{            
            string content = LoadFile(filepath);
            string decrypted = Libs.SecLib.Encryption.PlainText.Decrypt(content, _key);
            SaveFile(filepath, decrypted);
        }
        catch(Exception ex)
        {
            ShowError(ex.Message);
        }
    }    

    private static void ShowCommandValue(string command){
        string obfuscatedCommand = _commands.Get(command);        
        BashResult results = bash.Command(command);
        ShowMessage($"'{command}' -> '{obfuscatedCommand}'");
    }

    private static void ClearTerminal(){
        ShowDebug("ClearTerminal()");
        Console.Clear();
    }

     private static void ShowMessage(string message){
        Terminal.WriteLine(message, _settings.Colors.Default);
    }
    private static void ShowMessage(string message, LineColors colors){
        Terminal.WriteLine(message, colors);
    }
    private static void AddMessage(string message){
        Terminal.Write(message, _settings.Colors.Default);
    }
    private static void AddMessage(string message, LineColors colors)
    {
        Terminal.Write(message, colors);
    } 
    private static void AddEndMessage(string message, LineColors colors){
        Terminal.Write(message, colors, true);
    }        
    private static void ShowCommand(string message){
        Terminal.WriteLine(message, _settings.Colors.Commands);
    }  
    private static void ShowUpdate(string message){
        Terminal.OverWrite(message, _settings.Colors.Updates);
    }   
    private static void ShowUpdate(string message, LineColors colors){
        Terminal.OverWrite(message, colors);
    }                
    private static void ShowDevider(string message, LineColors colors){
        Terminal.WriteDevider(message, colors);
    }  
    private static void ShowDevider(string message ){
        Terminal.WriteDevider(message, _settings.Colors.Spacers);
    }      
    private static void ShowWarning(string message){
        Terminal.WriteLine(message, _settings.Colors.Warnings);
    }
    private static void ShowError(string message){
        Terminal.WriteLine(message, _settings.Colors.Errors);
    }
    private static void ShowDebug(string message)
    {
        if (_debugging)
            Terminal.WriteLine(message, _settings.Colors.Debugging);
    }
    

}