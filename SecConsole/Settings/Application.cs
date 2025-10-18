using Libs;

namespace SecConsole.Settings
{
    public class Application
    {
        public Application() { }

        public Colors Colors { get; set; } = new Colors();
        public Terminal Terminal { get; set; } = new Terminal();
     }
}