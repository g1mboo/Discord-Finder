using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DiscordFinding.Model
{
    interface IMessage
    {
        void WriteMessage(string message, Color color);
    }
}
