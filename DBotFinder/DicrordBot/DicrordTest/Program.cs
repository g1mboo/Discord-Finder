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
            Bot bot = new Bot(@"config.xml", "NzI1MDUyNDk5NjYwMzc0MDc3.XvJIlA.n3GnZkf-pCY99qvTIBq3Gqm3cuA");
            bot.ActivateBot().GetAwaiter().GetResult();
        }
    }
}
