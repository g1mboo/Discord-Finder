using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchBot.Commands
{
    interface IDiscordMessage
    {
        Task SendMessage(CommandContext context, string message);        
        Task SendPromptMessage(CommandContext context, string message);        
    }
}
