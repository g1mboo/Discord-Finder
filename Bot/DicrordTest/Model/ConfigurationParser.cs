using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SearchBot
{
    public class ConfigurationParser
    {
        private string path;
        private XDocument document;

        public ConfigurationParser(string path)
        {
            document = XDocument.Load(path);
            this.path = path;
        }

        public string GetPartConfig(string elementName)
        {
            XElement root = document.Root;
            return root.Element(elementName).Value;
        }

    }
}
