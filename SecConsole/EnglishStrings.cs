namespace SecConsole;

public class EnglishStrings : LocalizedStringsBase
{  
    public EnglishStrings(){

        Add("MenuOptionExit", "exit");
        Add("MenuOptionQuit", "quit");
        Add("MenuOptionMenu", "menu");
        Add("MenuOptionSettings", "options");
        Add("MenuOptionConvert", "convert");
        Add("MenuOptionRevert", "revert");
        Add("MenuOptionHashFile", "hash");
        Add("MenuOptionTest", "test");
        Add("MenuOptionLoadSettings", "Load Settings");
        Add("MenuOptionSaveSettings", "Save Settings");
        Add("DescriptionName", "Your name will be used in place of sudo");
        Add("PromptName", "Enter your name");
        Add("StringName", "name");
        Add("DescriptionKey", "Be sure to write down your security key. There is no way to retrieve it.");
        Add("PromptKey", "Enter your security key:");        
        Add("StringKey", "Key");
        Add("KeyValid", "Key accepted!");
        Add("KeyInvalid", "Key does not match!");
        Add("StringCommand", "Command");
        Add("ApplicationName", "Secure Console");
        Add("Copyright", "Copyright© Lloyd Bell 2025");
        Add("ApplicationDescription", "Custom console that scrambles the shell commands.");
        Add("FirstRunMessage", "Please wait while the system commands are obscifcated.");
        Add("SettingsSaveSuccess", "Settings Saved.");
        Add("SettingsSaveFailed", "Failed to save settings.");
        Add("SettingsLoadFailed", "Failed to load settings.");
    }

    new public string Language { get => "English"; }
}
