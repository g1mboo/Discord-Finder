using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchBot
{
    public struct BotConfiguration
    {

        //Настройки из файла
        public static string MessageTrigger { get; private set; }
        public static string StartPhrase { get; private set; }
        public static string CommandPrefix { get; private set; }
        public static bool PromptsParam { get; private set; }

        //Настройки по ходу программы
        public static string Token { get; private set; }
        public static bool StartedParam { get; set; }
        public static ulong ChannelId { get; set; }

        public static void Configurate(string messageTrigger, string startPhrase, string commandPrefix, bool promptsParam,  string token)
        {
            PromptsParam = promptsParam;
            MessageTrigger = messageTrigger;
            StartPhrase = startPhrase;
            CommandPrefix = commandPrefix;
            Token = token;
            StartedParam = false;
            ChannelId = default;
        }
       
    }
}
