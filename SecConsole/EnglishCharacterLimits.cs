    namespace SecConsole;

public class EnglishCharacterLimits : CharacterLimitBase
{  
    public EnglishCharacterLimits(){

        Add("NameMin", 3);
        Add("NameMax", 6);
        Add("KeyMin", 5);
        Add("KeyMax", 45);
    }

    new public string Language { get => "English"; }
}
