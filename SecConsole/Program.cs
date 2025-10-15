using System;
using System.Formats.Asn1;
using Libs;

namespace SecConsole;

class Program
{
    static private int _exitCode = 0;
    static private string _key = "thisisareallylongkey";
    static private LineColors _defaultColors = new LineColors(ConsoleColor.Green, ConsoleColor.Black);
    static private LineColors _commandColors = new LineColors(ConsoleColor.DarkGreen, ConsoleColor.Black);
    static private LineColors _spacerColors = new LineColors(ConsoleColor.Black, ConsoleColor.DarkGreen);
    static private LineColors _warningColors = new LineColors(ConsoleColor.Yellow, ConsoleColor.Black);
    static private LineColors _errorColors = new LineColors(ConsoleColor.DarkRed, ConsoleColor.Black);
    static private LineColors _debugColors = new LineColors(ConsoleColor.Blue, ConsoleColor.Black);
    static private string _results = "";
    static private string _workingDirectory = "";
    static private string _fileDirectory = "/usr/bin/";
    static private string _settingsFile = "settings.json";
    static private Settings _settings = new();
    static private Bash bash = new Bash();
    static private Libs.ColorTerminal Terminal = new ColorTerminal(_defaultColors);
    static private Scramble _scramble = new Scramble("");
    static private Libs.Shells.Bash.Shell _shell = new();
    static private CommandBuffer _buffer = new CommandBuffer();
    static private int _defaultDelay = 25;
    static private bool _isFirstRun = true;
    static private bool _debugging = true;

    public static void Main(string[] args)
    {
        Terminal = new(_defaultColors);
        EnglishStrings _strings = new EnglishStrings();
        EnglishCharacterLimits _limits = new EnglishCharacterLimits();
        _settings = new Settings();
        bash = new Bash();
        IsFirstRun();
        System.Threading.Thread.Sleep(_defaultDelay);
        if (_isFirstRun)
            _results = _shell.RunCommand("clear");

        Terminal.WriteLine("#", _spacerColors);
        Terminal.WriteCentered(_strings.Get("ApplicationName"));
        Terminal.WriteCentered(_strings.Get("Copyright"));
        Terminal.WriteLine(" ");
        Terminal.WriteCentered(_strings.Get("ApplicationDescription"));

        if (_isFirstRun)
        {
            Terminal.Write($"Kernal: {_shell.RunCommand("uname", "-s")}");
            Terminal.Write($"Hostname: {_shell.RunCommand("uname", "-n")}");
            Terminal.Write($"CPU: {_shell.RunCommand("uname", "-p")}");
            Terminal.Write($"OS: {_shell.RunCommand("uname", "-o")}");
        }
        Terminal.WriteLine("#", _spacerColors);

        LoadSettings(_strings);
        /*
        if (_isFirstRun)
            Terminal.Write(_strings.Get("DescriptionName"), _warningColors);
        tmp = "";    
        while(tmp == "")
            tmp = ShowPrompt(_strings.Get("PromptName"), _strings.Get("StringName"), _limits.Get("NameMin"), _limits.Get("NameMax"), ['1','2','3','4','5','6','7','8','9','0','!','@','#','$','%','^','&','*','(',')','-','=','-','+']);                      
        Commands.Update("sudo", tmp);
        */

        if (_key == "")
        {
            if (_isFirstRun)
                Terminal.Write(_strings.Get("DescriptionKey"), _warningColors);
            _key = GetKey(_strings, _limits);
        }

        _scramble = new(_key);

        if (_isFirstRun)
        {
            Terminal.Write(_strings.Get("FirstRunMessage"));
            ObsificateFileNames();
        }
        else
        {
            bool passed = TestKey(_key);
            while (!passed)
            {
                Terminal.Write(_strings.Get("KeyInvalid"));
                _key = GetKey(_strings, _limits);
                passed = TestKey(_key);
            }

            Terminal.Write(_strings.Get("KeyValid"));
        }

        ShowWorkingDirectory();

        
        while (_exitCode == 0)
        {
            _results = Terminal.Prompt();
            // check for arrow keys
            if (_results == Constants.UPARROW)
            {
                Terminal.Input = _buffer.Previous();
                continue;
            }
            else if (_results == Constants.DOWNARROW)
            {
                Terminal.Input = _buffer.Next();
                continue;
            }
            // handle special commands
            if (_results.StartsWith($"{_strings.Get("MenuOptionHashFile")} "))
            {
                string[] parts = _results.Split(" ");
                HashFile(parts[1]);
                continue;
            } 

            _results = BufferCommand(_results);
            if (_results == _strings.Get("MenuOptionExit") || _results == _strings.Get("MenuOptionQuit"))
            {
                RevertNames();
                _exitCode = 1;
            }
            else if (_results == _strings.Get("MenuOptionMenu"))
            {
                DisplayMenu(_strings);
            }
            else if (_results == _strings.Get("MenuOptionSettings"))
            {
                // ToDo: Launch settings display
            }
            else if (_results == _strings.Get("MenuOptionRevert"))
            {
                RevertNames();
            }
            else if (_results == _strings.Get("MenuOptionConvert"))
            {
                ObsificateFileNames();
            }
            else if (_results == _strings.Get("MenuOptionTest"))
            {
                TestKey(_key);
            }           
            else if (_results == _strings.Get("MenuOptionLoadSettings"))
            {
                LoadSettings(_strings);
            }
            else if (_results == _strings.Get("MenuOptionSaveSettings"))
            {
                SaveSettings(_strings);
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

    private static string GetKey(ILocalizedStrings strings, ICharacterLimits limits)
    {
        string tmp = "";
        while (tmp == "")
            tmp = ShowPrompt(strings.Get("PromptKey"), strings.Get("StringKey"), limits.Get("KeyMin"), limits.Get("KeyMax"), ['!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=', '-', '+']);
        System.Threading.Thread.Sleep(500);
        return tmp;
    }

    private static void DisplayMenu(ILocalizedStrings strings)
    {
        Terminal.Write(_shell.RunCommand("clear", ""));
        Terminal.Write($"{strings.Get("StringCommand")}:'{strings.Get("MenuOptionMenu")}' - Display the menu.");
        Terminal.Write($"{strings.Get("StringCommand")}:'{strings.Get("MenuOptionExit")}' or '{strings.Get("MenuOptionQuit")}' - Exit the console.");
        Terminal.Write($"{strings.Get("StringCommand")}:'{strings.Get("MenuOptionSettings")}' - Edit the console settings.");
        Terminal.Write($"{strings.Get("StringCommand")}:'{strings.Get("MenuOptionConvert")}' - Obsificate the commands.");
        Terminal.Write($"{strings.Get("StringCommand")}:'{strings.Get("MenuOptionRevert")}' - Revert to non obsfucated.");
        Terminal.Write($"{strings.Get("StringCommand")}:'{strings.Get("MenuOptionTest")}' - Test to make sure commands are obsfucated.");
        Terminal.Write($"{strings.Get("StringCommand")}:'{strings.Get("MenuOptionLoadSettings")}' - ");
        Terminal.Write($"{strings.Get("StringCommand")}:'{strings.Get("MenuOptionSaveSettings")}' - ");
        Terminal.Write($"{strings.Get("StringCommand")}:'{strings.Get("MenuOptionHashFile")}' - ");
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
        BashResult results = bash.Command($"cd \"{target}\"");
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
            Terminal.OverWrite("", _defaultColors);
            return true;
        }
        if (fullCommand == "ls")
        {
            string command = $"{Commands.Get("ls")} -a '{GetWorkingDirectory()}'";
            //Terminal.Write($"Command = {command}", _defaultColors);
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
                string command = $"{Commands.Get("ls")}";
                string flags = "";
                string filepath = GetWorkingDirectory();
                string[] parts = fullCommand.Split(" ");
                // Check for flags
                if (parts[1].StartsWith("-"))
                {
                    flags = parts[1];
                    if (parts.Length > 2)
                        filepath = parts[2];
                }
                else
                {
                    filepath = parts[1];
                }
                command += flags.Count() > 0 ? $" {flags} " : "";
                command += $"{filepath}";
                ShowDebug($"Command = {command}");
                BashResult results = bash.Command(command);
        }
        if (fullCommand == "swd")
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
        string tmp = Commands.Get(fullCommand);
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
            string tmpPart = Commands.Get(parts[i]);
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
        var list = Commands.GetList();
        int total = Commands.Count();
        int count = 0;
        string findCommand = Commands.Get("find");
        foreach (string command in list)
        {
            string obfuscatedCommand = Commands.Get(command);
            bool success = FindFile(obfuscatedCommand);
            if (success)
                count++;
        }
        ShowMessage($"{count}/{total} matches found!");
        return count > 0;
    }

    private static bool FindFile(string fileName)
    {
        //string findCommand = Commands.Get("find");
        string findCommand = "find";
        var results = bash.Command($"{_fileDirectory}{findCommand} {_fileDirectory}{fileName}");
        System.Threading.Thread.Sleep(_defaultDelay);
        return results.Output.Length > 0;
    }

    private static void ObsificateCommandNames()
    {
        foreach (string command in Commands.GetList())
        {
            string obsfucated = Commands.Get(command);
            if (obsfucated == "")
            {
                obsfucated = _scramble.HashString(command);
                Commands.Update(command, obsfucated);
            }
        }
    }

    private static void ObsificateFileNames()
    {
        ObsificateCommandNames();
        Terminal.Write($"Converting {Commands.Count()} system commands.", _defaultColors);
        foreach (string command in Commands.GetList())
        {
            bool originalFound = FindFile(command);
            string obsfucated = Commands.Get(command);
            if (originalFound)
            {                
                bash.Mv($"{_fileDirectory}{command}", $"{_fileDirectory}{obsfucated}");
                // Test for success
                bool success = FindFile(obsfucated);
                string successString = success ? "Done" : "Failed";
                LineColors useColors = success ? _defaultColors : _warningColors;
                Terminal.OverWrite($"{successString} Converting: {command} -> {obsfucated}", useColors);
                if (!success)
                {
                    Commands.Update(command, command);
                }
            }
            else
            {
                bool obsfucatedFound = FindFile(obsfucated);
                if (!obsfucatedFound)
                {
                    ShowError($"{command} Not Found!");
                    Commands.Remove(command);
                }
                else
                {
                    Terminal.OverWrite($"{command} Found!", _defaultColors);
                    //Commands.Update(command, obsfucated);
                }
            }
        }
        Terminal.OverWrite($"Convertion Complete!", _defaultColors);

    }

    private static void RevertNames()
    {
        Terminal.Write($"Reverting {Commands.Count()} system commands.", _defaultColors);
        foreach (string command in Commands.GetList())
        {
            string currentCommand = Commands.Get(command);
            bash.Mv($"{_fileDirectory}{currentCommand}", $"{_fileDirectory}{command}");
            Commands.Update(command, command);
            Terminal.OverWrite($"Reverting: {currentCommand} -> {command}", _debugColors);
            System.Threading.Thread.Sleep(_defaultDelay);
        }
        Terminal.OverWrite($"Revertion Complete!", _defaultColors);
    }

    private static string ShowPrompt(string prompt, string value, int minChars, int maxChars, char[] omit)
    {
        string input = Terminal.Prompt(prompt);
        if (string.IsNullOrWhiteSpace(input))
        {
            Terminal.Write($"{value} can not be blank.", _errorColors);
            return "";
        }
        if (input != null && input.Length < minChars)
        {
            Terminal.Write($"{value} should be at least {minChars} characters.", _errorColors);
            return "";
        }
        if (input != null && input.Length > maxChars)
        {
            Terminal.Write($"{value} should be no more then {maxChars} characters.", _errorColors);
            return "";
        }

        for (int i = 0; i < omit.Count(); i++)
        {
            string s = omit[i].ToString();
            if (input.Contains(s))
            {
                Terminal.Write($"{value} can not contain the following character {s}.", _errorColors);
                return "";
            }
        }
        return input;
    }

    private static void SaveSettings(ILocalizedStrings strings)
    {
        try{
            string settings = _settings.ToJson();
            bool success = SaveFile(_settingsFile, settings);
            ShowMessage(strings.Get("SettingsSaveSuccess"));
        }
        catch(Exception ex)
        {
            ShowError($"{strings.Get("SettingsLoadFailed")} - {ex.Message}");
        }        
    }

    private static void LoadSettings(ILocalizedStrings strings)
    {
        try{
            ShowDebug("LoadSettings()");
            string settings = _settings.ToJson();
            ShowDebug(settings);
            string results = LoadFile(_settingsFile);
            ShowDebug(results);
            ShowMessage("Settings Loaded");
        }
        catch(Exception ex)
        {
            ShowMessage("Settings Failed To Load");
        }
    }

    private static string LoadFile(string filepath)
    {
        ShowDebug($"LoadFile( {filepath} )");
        string command = Commands.Get("cat");
        BashResult results = bash.Command($"{command} {filepath}");
        ShowDebug($"Loading file {filepath} - ExitCode = {results.ExitCode}");
        if(results.ExitCode != 0){
            throw new Exception(results.ErrorMsg);
        }    
        return results.Output;
    }

    private static bool SaveFile(string filepath, string content)
    {
        string command = Commands.Get("echo");
        string settings = _settings.ToJson();
        BashResult results = bash.Command($"{command} {content} > {filepath}");
        if(results.ExitCode != 0){
            throw new Exception(results.ErrorMsg);
        }
        return true;
    }    

    private static string BufferCommand(string command)
    {
        command = command.TrimEnd();
        command = command.TrimEnd(Environment.NewLine.ToCharArray());
        _buffer.Add(command);
        Terminal.OverWrite($"{Terminal.TerminalString}{_buffer.Last()}", _commandColors);
        return command;
    }

    private static void HashFile(string filepath){
        try{
            string results = LoadFile(filepath);
            string hashed = _scramble.HashString(results);
            SaveFile(filepath, hashed);
        }
        catch(Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private static void UnHashFile(string filepath){
        try{
            string results = LoadFile(filepath);
            string hashed = _scramble.HashString(results);
            SaveFile(filepath, hashed);
        }
        catch(Exception ex)
        {
            ShowError(ex.Message);
        }
    }    


    private static void ShowDebug(string message){
        if(_debugging)
            Terminal.Write(message, _debugColors);
    }
    private static void ShowMessage(string message){
        Terminal.Write(message, _defaultColors);
    }
    private static void ShowWarning(string message){
        Terminal.Write(message, _warningColors);
    }
    private static void ShowError(string message){
        Terminal.Write(message, _errorColors);
    }

}