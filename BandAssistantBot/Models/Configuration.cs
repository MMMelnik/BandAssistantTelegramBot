namespace BandAssistantBot.Models
{
    public class Configuration
    {
        public static readonly string BotToken = "1043288032:AAGtCYJBpZQW_7a8mndbNt2fR11yOhIj66o";

#if USE_PROXY
        public static class Proxy
        {
            public readonly static string Host = "{PROXY_ADDRESS}";
            public readonly static int Port = 8080;
        }
#endif
    }
}
