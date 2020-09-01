using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiscordFinding.Dialogs.Bot_settings
{
    public class StreamBotXmlConfig
    {
        private string path;
        private XDocument document;

        public StreamBotXmlConfig(string path)
        {            
            document = XDocument.Load(path);
            this.path = path;
        }

        public string GetPartConfig(string elementName)
        {            
            XElement root = document.Root;
            return root.Element(elementName).Value;            
        }

        public void SetPartConfig(string elementName, string value)
        {           
            XElement root = document.Root;
            root.SetElementValue(elementName, value);
            document.Save(path);
        }
    }
}
