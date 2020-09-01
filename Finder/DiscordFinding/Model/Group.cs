using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordFinding.Model
{
    public class Group : ICloneable
    {
        public string Name { get; set; }
        public List<string> Students { get; set; }

        public Group() 
        {
            Name = string.Empty;
            Students = new List<string>();
        }

        public Group(string name)
        {
            Name = name;
            Students = new List<string>();
        }

        public Group(string name, List<string> students)
        {
            Name = name;
            Students = students;
            Students.Sort();
        }

        public object Clone()
        {
            Group group = new Group();
            group.Name = Name;
            group.Students = Students;
            return group; 
        }
    }
}
