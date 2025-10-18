using System;
using System.Text;
using System.IO;
using System.Formats.Asn1;
using System.Text.Json;
using System.Security.Cryptography;
using Libs;

namespace SecConsole;

class Program
{
    static private int _exitCode = 0;
    static private string _key = "thisisareallylongkey";
    static private string _results = "";
    static private string _workingDirectory = "";
    static private string _fileDirectory = "/usr/bin/";
    static private string _settingsFile = "settings.json";
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
        LoadSettings();
        Terminal = new(_settings.Colors.Default);
        EnglishStrings _strings = new EnglishStrings();
        EnglishCharacterLimits _limits = new EnglishCharacterLimits();
        
        bash = new Bash();
        IsFirstRun();
        System.Threading.Thread.Sleep(_defaultDelay);
        if (_isFirstRun)
            _results = _shell.RunCommand("clear");

        ShowDevider(" ");
        Terminal.WriteCentered(_strings.Get("ApplicationName"));
        Terminal.WriteCentered(_strings.Get("Copyright"));
        Terminal.WriteCentered(_strings.Get("ApplicationDescription"));
        ShowDevider(" ");

        if (_isFirstRun)
        {
            ShowMessage($"Kernal: {_shell.RunCommand("uname", "-s")}");
            ShowMessage($"Hostname: {_shell.RunCommand("uname", "-n")}");
            ShowMessage($"CPU: {_shell.RunCommand("uname", "-p")}");
            ShowMessage($"OS: {_shell.RunCommand("uname", "-o")}");
        }
        ShowDevider(" ");

        
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
                Terminal.Input = _buffer.Previous();
                continue;
            }
            else if (_results == Constants.DOWNARROW)
            {
                Terminal.Input = _buffer.Next();
                continue;
            }

            _results = BufferCommand(_results);
            // handle special commands
            if (_results.StartsWith($"{_strings.Get("MenuOptionEncryptFile")}"))
            {
                string[] parts = _results.Split(" ");
                EncryptFile(parts[1]);
                continue;
            } 
            else if (_results.StartsWith($"{_strings.Get("MenuOptionDecryptFile")}"))
            {
                string[] parts = _results.Split(" ");
                DecryptFile(parts[1]);
                continue;
            } 
            // Handle menu options
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
                LoadSettings();
            }
            else if (_results == _strings.Get("MenuOptionSaveSettings"))
            {
                if(SaveSettings())
                    ShowMessage(_strings.Get("SettingsSaveSuccess"));
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
        LineColors firstColor = new LineColors(ConsoleColor.Blue, ConsoleColor.Black);
        LineColors secondColor = new LineColors(ConsoleColor.White, ConsoleColor.Black); ;
        int count = 1;
        string seperator = " - ";
        //Terminal.Write(_shell.RunCommand("clear", ""));
        ShowDevider(" ", new LineColors(ConsoleColor.DarkBlue, ConsoleColor.DarkBlue));
        Terminal.WriteCentered("Menu");
        ShowDevider(" ", new LineColors(ConsoleColor.DarkBlue, ConsoleColor.DarkBlue));

        AddMessage($"{count++}. ", secondColor);
        AddMessage($"{strings.Get("MenuOptionQuit")}{seperator}", firstColor);
        AddEndMessage("Exit the console.", secondColor);

        AddMessage($"{count++}. ", secondColor);
        AddMessage($"{strings.Get("MenuOptionConvert")}{seperator}", firstColor);
        AddEndMessage("Obsificate the commands.", secondColor);

        AddMessage($"{count++}. ", secondColor);
        AddMessage($"{strings.Get("MenuOptionRevert")}{seperator}", firstColor);
        AddEndMessage("Revert to non obsfucated.", secondColor);
        
        AddMessage($"{count++}. ", secondColor);
        AddMessage($"{strings.Get("MenuOptionLoadSettings")}{seperator}", firstColor);
        AddEndMessage("Loads the settings.", secondColor);

        AddMessage($"{count++}. ", secondColor);
        AddMessage($"{strings.Get("MenuOptionSaveSettings")}{seperator}", firstColor);
        AddEndMessage("Saves the settings.", secondColor);

        AddMessage($"{count++}. ", secondColor);
        AddMessage($"{strings.Get("MenuOptionEncryptFile")}{seperator}", firstColor);
        AddEndMessage("Encrypt a file.", secondColor);

        AddMessage($"{count++}. ", secondColor);
        AddMessage($"{strings.Get("MenuOptionDecryptFile")}{seperator}", firstColor);
        AddEndMessage("Decrypt a file.", secondColor);

        AddMessage($"{count++}. ", secondColor);
        AddMessage($"{strings.Get("MenuOptionSettings")}{seperator}", firstColor);
        AddEndMessage("Edit the console settings.", secondColor);

        AddMessage($"{count++}. ", secondColor);
        AddMessage($"{strings.Get("MenuOptionTest")}{seperator}", firstColor);
        AddEndMessage("Test to make sure commands are obsfucated.", secondColor);

        ShowDevider(" ", new LineColors(ConsoleColor.DarkBlue, ConsoleColor.DarkBlue));
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
            Terminal.ClearLine();
            return true;
        }
        if (fullCommand == "ls")
        {
            string command = $"{Commands.Get("ls")} -a '{GetWorkingDirectory()}'";
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
        ShowMessage($"Converting {Commands.Count()} system commands.");
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
                LineColors useColors = success ? _settings.Colors.Updates : _settings.Colors.Warnings;
                ShowUpdate($"{successString} Converting: {command} -> {obsfucated}", useColors);
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
                    ShowUpdate($"{command} Not Found!");
                    Commands.Remove(command);
                }
                else
                {
                    ShowUpdate($"{command} Found!");
                    //Commands.Update(command, obsfucated);
                }
            }
        }
        ShowUpdate($"Convertion Complete!");

    }

    private static void RevertNames()
    {
        ShowMessage($"Reverting {Commands.Count()} system commands.");
        foreach (string command in Commands.GetList())
        {
            string currentCommand = Commands.Get(command);
            bash.Mv($"{_fileDirectory}{currentCommand}", $"{_fileDirectory}{command}");
            Commands.Update(command, command);
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
            //string jsonString = JsonSerializer.Serialize(_settings);
            string jsonString = JsonSerializer.Serialize(new Settings.Terminal());
            ShowDebug($"jsonString = {jsonString}");
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
            ShowDebug("LoadSettings()");
            string jsonString = LoadFile(_settingsFile);
            Settings.Application? settings = JsonSerializer.Deserialize<Settings.Application>(jsonString);
            if(settings == null)
                _settings = new Settings.Application();
            _settings = settings;
        }
        catch(Exception ex)
        {
            ShowDebug("Failed to load file.");
            _settings = new Settings.Application();
            SaveSettings();
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
        try
        {
            File.WriteAllText(filepath, content);
        }
        catch
        {
            throw new Exception($"Failed to save file {filepath}");
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
            ShowDebug($"EncryptFile( {filepath} )");
            string content = LoadFile(filepath);
            ShowDebug($"content = {content}");
            string encrypted = StringEncrypt.Encrypt(content, _key);
            SaveFile(filepath, encrypted);
        }
        catch(Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private static void DecryptFile(string filepath){
        try{            
            ShowDebug($"DecryptFile( {filepath} )");
            string content = LoadFile(filepath);
            ShowDebug($"content = {content}");
            string decrypted = StringEncrypt.Decrypt(content, _key);
            SaveFile(filepath, decrypted);
        }
        catch(Exception ex)
        {
            ShowError(ex.Message);
        }
    }    

    private static void UnEncryptFile(string filepath){
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