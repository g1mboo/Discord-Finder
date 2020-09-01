using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SearchBot.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchBot.Commands
{
    public class Commands : IDiscordMessage
    { 
        [Command("start")]
        [RequireRolesAttribute("Admin")]
        [Description("Включает запись пользователей в лог.\r\nЗаписи будут производится исключительного из данного канала")]
        public async Task Start(CommandContext ctx)
        {
            if (!BotConfiguration.StartedParam)
            {
                await SendPromptMessage(ctx, "Запись на данном канале запущена");
                await SendMessage(ctx, $"{BotConfiguration.StartPhrase} [{BotConfiguration.MessageTrigger}]");

                //Авторизируем дискорд канал
                BotConfiguration.ChannelId = ctx.Channel.Id;

                //Отключаем повторную авторизацию канала
                BotConfiguration.StartedParam = false;
            }
            else
                await SendPromptMessage(ctx, $"Запись уже запущена");
        }

        [Command("clear")]
        [RequireRolesAttribute("Admin")]
        [Description("Очищает лог от записей.\r\nНе влияет на запись.\r\nНе зависит от конкретного канала")]
        public async Task Clear(CommandContext ctx)
        {            
            if(File.Exists("students.log"))
                File.WriteAllText("students.log", "");
            
            await SendPromptMessage(ctx, "Лог очищен");
        }

        [Command("stop")]
        [RequireRolesAttribute("Admin", "Administrator", "Администратор", "Админ")]
        [Description("Выключает запись пользователей в лог.\r\nПоследущие пользователи не будут записаны(на любом канале), пока запись не будет включена")]
        public async Task Stop(CommandContext ctx)
        {  
            BotConfiguration.ChannelId = default;
            BotConfiguration.StartedParam = false;
            await SendPromptMessage(ctx, "Запись на этом канале отключена");            
        }

        [Command("random")]
        [Description("Выдает псевдослучайное число в определенном диапазоне")]
        public async Task Random(CommandContext ctx, 
            [Description("Начало диапазона")] int minValue,
            [Description("Конец диапазона")] int maxValue)
        {              
            Random random = new Random();
            string message = $"Пользователю ({AuthorParser.GetNickname(ctx.Message.Author)}) выпало число: {random.Next(minValue, maxValue)}";
            await SendMessage(ctx, message);
        }

        
        public async Task SendMessage(CommandContext context, string message)
        {
            await context.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }

        public async Task SendPromptMessage(CommandContext context, string message)
        {
            if (BotConfiguration.PromptsParam)
                await context.Channel.SendMessageAsync(message).ConfigureAwait(false);
        }
    }
}