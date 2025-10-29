using System;
using System.Text;
using System.IO;
using System.Formats.Asn1;
using System.Text.Json;
using System.Security.Cryptography;
using Libs;
using Libs.SecLib;
using Libs.SecLib.Encryption;

using InternalCommand = SecConsole.Commands.Internal.CommandType;

namespace SecConsole;

class Program
{
    static private int _exitCode = 0;
    static private string _key = "thisisareallylongkey";
    static private string _results = "";
    static private string _workingDirectory = "";
    static private string _fileDirectory = "/usr/bin/";
    static private string _settingsFile = "settings.json";
    static private Commands.System.CommandList _systemCommands = new ();
    static private Commands.Internal.CommandList _internalCommands = new ();
    static private Settings.Application _settings = new();
    static private Bash bash = new Bash();
    static private Libs.ColorTerminal Terminal = new ColorTerminal();
    static private Scramble _scramble = new Scramble("");
    static private Libs.Shells.Bash.Shell _shell = new();
    static private CommandBuffer _buffer = new CommandBuffer();
    static private int _defaultDelay = 10;
    static private bool _isFirstRun = true;
    static private bool _debugging = true;

    public static void Main(string[] args)
    {
        _internalCommands.Add(InternalCommand.MENU, "menu", "", "Displays the menu.");
        _internalCommands.Add(InternalCommand.BLUR, "blur", "", "Obsificates the shell commands.");
        _internalCommands.Add(InternalCommand.FOCUS, "focus", "", "Restores the shell commands.");
        _internalCommands.Add(InternalCommand.RUN, "run", "<filepath>", "Runs a shell script file.");
        _internalCommands.Add(InternalCommand.CONVERT, "convert", "<filepath>", "Converts a shell script file to use obsificated commands.");
        _internalCommands.Add(InternalCommand.ENCRYPT, "encrypt", "<filepath>", "Encrypts a file.");
        _internalCommands.Add(InternalCommand.DECRYPT, "decrypt", "<filepath>", "Decrypts a file.");
        _internalCommands.Add(InternalCommand.SHOW, "show", "<command>", "Displays the current value for a shell command.");
        _internalCommands.Add(InternalCommand.PAST, "past", "", "Show terminal history.");
        _internalCommands.Add(InternalCommand.TEST, "test", "", "Test Obsificated names.");
        _internalCommands.Add(InternalCommand.QUIT, "quit", "", "Quits the application");

        _systemCommands.Add("apt", "/usr/bin/");
        _systemCommands.Add("cat", "/usr/bin/");
        _systemCommands.Add("cd", "/usr/bin/");
        _systemCommands.Add("curl", "/usr/bin/");
        _systemCommands.Add("chfn", "/usr/bin/");
        _systemCommands.Add("chmod", "/usr/bin/");
        _systemCommands.Add("chown", "/usr/bin/");
        _systemCommands.Add("clear", "/usr/bin/");
        _systemCommands.Add("chsh", "/usr/bin/");
        _systemCommands.Add("ciptool", "/usr/bin/");
        _systemCommands.Add("cmake", "/usr/bin/");
        _systemCommands.Add("echo", "/usr/bin/");
        _systemCommands.Add("history", "/usr/bin/");
        _systemCommands.Add("id", "/usr/bin/");
        //_systemCommands.Add("find", "/usr/bin/"); 
        _systemCommands.Add("netstat", "/usr/bin/");
        _systemCommands.Add("last", "/usr/bin/");
        _systemCommands.Add("open", "/usr/bin/");
        _systemCommands.Add("passwd", "/usr/bin/");
        _systemCommands.Add("ping", "/usr/bin/");
        _systemCommands.Add("ps", "/usr/bin/");
        _systemCommands.Add("scp", "/usr/bin/");
        _systemCommands.Add("su", "/usr/bin/");
        _systemCommands.Add("sestatus", "/usr/bin/");
        _systemCommands.Add("ssh", "/usr/bin/");
        _systemCommands.Add("ssh-keygen", "/usr/bin/");
        _systemCommands.Add("ls", "/usr/bin/");
        _systemCommands.Add("top", "/usr/bin/");
        _systemCommands.Add("touch", "/usr/bin/");
        _systemCommands.Add("type", "/usr/bin/");
        _systemCommands.Add("ufw", "/usr/bin/");
        _systemCommands.Add("visudo", "/usr/bin/");
        _systemCommands.Add("wget", "/usr/bin/");
        _systemCommands.Add("whoami", "/usr/bin/");

        _systemCommands.Add("sudo", "/usr/bin/");
        _systemCommands.SetExplicit("sudo", true);
        _systemCommands.UpdateObscureName("sudo", "lloyd");

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
        _systemCommands.Update("sudo", tmp);
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

        ObsificateFileNames();
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
            if(_results.StartsWith(_internalCommands.Marker)){
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

    private static void HandleSpecialCommand(string fullCommand){
        string command = fullCommand;
        string option = "";
        int firstSpace = fullCommand.IndexOf(" ");
        if(firstSpace >= 0){
            command = fullCommand.Substring(0, firstSpace);
            option = fullCommand.Substring(firstSpace + 1);
        }
        command = command.Replace(_internalCommands.Marker, "");

        ShowDebug($"HandleSpecialCommand() command:{command} option:{option} firstSpace:{firstSpace}");
        Commands.Internal.CommandDetails? commandDetails = _internalCommands.GetFromCommand(command);
        if(commandDetails == null){
            ShowError("Command not found");
            return;
        }

            if(commandDetails.Id == InternalCommand.MENU)
            {
                DisplayMenu();
            }
            else if(commandDetails.Id == InternalCommand.BLUR)
            {
                ObsificateFileNames();
            }
            else if(commandDetails.Id == InternalCommand.FOCUS)
            {
                ResetFileNames();
            } 
            else if(commandDetails.Id == InternalCommand.RUN)
            {
                RunScriptFile(option);
            } 
            else if(commandDetails.Id == InternalCommand.CONVERT)
            {
                // TODO: Convert shell script
            }                         
            else if(commandDetails.Id == InternalCommand.ENCRYPT)
            {
                EncryptFile(option);
            } 
            else if(commandDetails.Id == InternalCommand.DECRYPT)
            {
                DecryptFile(option);
            } 
            else if(commandDetails.Id == InternalCommand.SHOW)
            {
                ShowCommandValue(option);
            } 
            else if(commandDetails.Id == InternalCommand.TEST)
            {
                TestKey(_key);
            }  
            else if(commandDetails.Id == InternalCommand.PAST)
            {
                DisplayCommandHistory();
            }                         
            else if(commandDetails.Id == InternalCommand.QUIT)
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
        string seperator = ": ";

        AddMessage($"    ", secondColor);
        AddMessage($"Kernal{seperator}", firstColor);
        AddEndMessage($"{bash.Command("uname -s").Output}", secondColor);

        AddMessage($"    ", secondColor);
        AddMessage($"Hostname{seperator}", firstColor);
        AddEndMessage($"{bash.Command("uname -n").Output}", secondColor);

        AddMessage($"    ", secondColor);
        AddMessage($"CPU{seperator}", firstColor);
        AddEndMessage($"{bash.Command("uname -p").Output}", secondColor);

        AddMessage($"    ", secondColor);
        AddMessage($"OS{seperator}", firstColor);
        AddEndMessage($"{bash.Command("uname -o").Output}", secondColor);                

    }

    private static void DisplayMenu()
    {
        ClearTerminal();
        LineColors firstColor = _settings.Colors.Default;
        LineColors secondColor = _settings.Colors.Commands;
        LineColors deviderColor = new LineColors(_settings.Colors.Commands.TextColor, _settings.Colors.Commands.TextColor);
        string marker = _internalCommands.Marker;
        ShowDevider(" ", deviderColor);
        Terminal.WriteCentered("Menu");
        ShowDevider(" ", deviderColor);

        foreach(Commands.Internal.CommandDetails cmd in _internalCommands.GetAll())
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
        string fullCommand = $"{_systemCommands.GetObscuredName("cd")} '{target}'";
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
                UpdateWorkingDirectory(fullCommand.Replace("cd ", ""));

            }
            ShowWorkingDirectory();
            Terminal.ClearLine();
            return true;
        }
        if (fullCommand == "ls")
        {
            string command = $"{_systemCommands.GetObscuredName("ls")} -a '{GetWorkingDirectory()}'";
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
            string command = $"{_systemCommands.GetObscuredName("ls")}";
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
        string tmp = _systemCommands.GetObscuredName(fullCommand);
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
            string tmpPart = _systemCommands.GetObscuredName(parts[i]);
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
        int total = _systemCommands.Count();
        int originalCount = 0;
        int obscureCount = 0;
        foreach (Commands.System.CommandDetails details in _systemCommands.GetAll())
        {
            bool success = false;
            success = FindFile(details.ObscurePath);
            if(success)
                obscureCount++;
            success = FindFile(details.OriginalPath);
            if(success)
                originalCount++;                
        }
        ShowMessage($"Original:{originalCount} Obscured:{obscureCount} --- {obscureCount} of {total} matches found!");
        return obscureCount > 0;
    }

    private static bool FindFile(string filepath)
    {
        //string findCommand = _systemCommands.GetObscuredName("find");
        string findCommand = $"{_fileDirectory}find {filepath}";
        BashResult results = bash.Command(findCommand);
        //ShowDebug($"FindFile( {filepath} ) - ExitCode:{results.ExitCode}");
        //ShowDebug($"    findCommand:{findCommand}");
        //ShowDebug($"    {results.Output}");
        System.Threading.Thread.Sleep(_defaultDelay);
        return results.Output.Length > 0;
    }

    private static void ObsificateFileNames()
    {
        // obsfucated = _scramble.HashString(command);
        int total = _systemCommands.Count();
        ShowMessage($"Converting {total} system commands.");
        foreach (Commands.System.CommandDetails details in _systemCommands.GetAll())
        {
            bool originalFound = FindFile(details.OriginalPath);
            if (originalFound)
            {                
                string obsfucated = details.ObscureName;
                if(!details.Explicit)
                    _scramble.HashString(details.OriginalName);
                _systemCommands.UpdateObscureName(details.OriginalName, obsfucated);
                bash.Mv($"{details.OriginalPath}", $"{details.ObscurePath}");
                // Test for success
                bool success = FindFile(details.ObscurePath);
                string successString = success ? "Done" : "Failed";
                LineColors useColors = success ? _settings.Colors.Updates : _settings.Colors.Warnings;
                ShowUpdate($"{successString} Obscuring: {details.OriginalName} -> {obsfucated}", useColors);
                if (!success)
                {
                    _systemCommands.UpdateObscureName(details.OriginalName, details.ObscureName);
                }
            }
            else
            {
                bool obsfucatedFound = FindFile(details.ObscurePath);
                if (!obsfucatedFound)
                {
                    ShowUpdate($"{details.OriginalName} Not Found!");
                    _systemCommands.Remove(details.OriginalName);
                }
                else
                {
                    ShowUpdate($"{details.OriginalName} Already obscured!");
                }
            }
        }
        ShowUpdate($"Convertion Complete!");

    }

    private static void ResetFileNames()
    {
        ShowMessage($"Reverting {_systemCommands.Count()} system commands.");
        foreach (Commands.System.CommandDetails details in _systemCommands.GetAll())
        {
            bool obsfucatedFound = FindFile(details.ObscurePath);
            if(obsfucatedFound){
                ShowUpdate($"Reverting: {details.ObscurePath} -> {details.OriginalPath}");
                bash.Mv($"{details.ObscurePath}", $"{details.OriginalPath}");
                _systemCommands.UpdateObscureName(details.OriginalName, details.OriginalName);
                System.Threading.Thread.Sleep(_defaultDelay);
            }
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
    private static void RunScriptFile(string filepath){
        try{            
            if(filepath.Contains("/") == false)
                filepath = $"{_workingDirectory}/{filepath}";
            //string content = LoadFile(filepath);
            //BashResult results = bash.Command(content);
            BashResult results = bash.Command($". '{filepath}'");
            if (results.ExitCode == 0)
            {
                ShowMessage(results.Output);
            }
            else{
                ShowError($"Failed to run file. ExitCode:{results.ExitCode}");
            }
        }
        catch(Exception ex)
        {
            ShowError(ex.Message);
        }
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
        string obfuscatedCommand = _systemCommands.GetObscuredName(command);        
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