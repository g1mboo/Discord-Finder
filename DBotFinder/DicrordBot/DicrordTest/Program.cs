using DSharpPlus;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SearchBot;

namespace SrchBot
{
    class Program
    {        
        static void Main(string[] args)
        {
            Bot bot = new Bot(@"config.xml", "token");
            bot.ActivateBot().GetAwaiter().GetResult();
        }
    }
}
