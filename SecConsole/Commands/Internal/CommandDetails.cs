namespace SecConsole.Commands.Internal
{
    public class CommandDetails
    {
        public CommandDetails(CommandType id, string command, string options, string description)
        {
            Id = id;
            Command = command;
            Options = options;
            Description = description;
        }
        public CommandType Id { get; set; } = CommandType.UNKNOWN;
        public string Command { get; set; } = "";
        public string Options { get; set; } = "";
        public string Description { get; set; } = "";
    }
}