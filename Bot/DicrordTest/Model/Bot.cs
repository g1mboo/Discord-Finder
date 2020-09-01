using DSharpPlus;
using DSharpPlus.CommandsNext;
using SearchBot.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SearchBot
{
    public class Bot    
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextModule Commands { get; private set; }        

        public Bot(string path, string token)
        {            
            ConfigurationParser configurationParser = new ConfigurationParser(path);

            //Конфигурация
            BotConfiguration.Configurate(
                configurationParser.GetPartConfig("MessageTrigger"),
                configurationParser.GetPartConfig("StartPhrase"),
                configurationParser.GetPartConfig("CommandPrefix"),
                configurationParser.GetPartConfig("PromptsParam") == "true" ? true : false,
                token);              
                         
        }

        public async Task ActivateBot()
        {
            //Инициализируем бота
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = BotConfiguration.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });            

            //Добавляем человека в лог
            Client.MessageCreated += async e =>
            {
                if (e.Message.Content.StartsWith(BotConfiguration.MessageTrigger) & e.Message.ChannelId == BotConfiguration.ChannelId)
                {                    
                    await AddingStudentsToLog(AuthorParser.GetNickname(e.Message.Author));
                    await Task.Run(() => Console.WriteLine(e.Message.Author.ToString()));
                }
            };

            //Инициализируем команды бота
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefix = BotConfiguration.CommandPrefix,
                EnableDms = false,
                EnableMentionPrefix = true
            };
            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<Commands.Commands>();

            //Запускаем бота
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }        

        private static async Task AddingStudentsToLog(string name)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("students.log", true))                
                    await writer.WriteLineAsync(name).ConfigureAwait(false);               
            }
            catch { }
        }
    }
}
