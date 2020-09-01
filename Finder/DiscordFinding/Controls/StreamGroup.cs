using DiscordFinding.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DiscordFinding
{
    public class StreamGroup
    {
        private XDocument document;

        public static string Path { get; set; }        

        public List<Group> GetGroups()
        {
            document = XDocument.Load(Path);
            XElement root = document.Root;
            
            List<Group> groups = new List<Group>();

            foreach (XElement groupElem in root.Elements().ToList())
            {
                Group group = new Group() { Name = groupElem.Attribute("name").Value };

                //Собираем всех studentElement в group.Students
                foreach (XElement studentElement in groupElem.Elements().ToList())
                    group.Students.Add(studentElement.Element("SecondName").Value + " " + studentElement.Element("Name").Value);

                //Сортируем все элементы
                group.Students.Sort();

                //Добавляем group в groups
                groups.Add(group);
            }

            return groups;
        }

        public bool SetGroups(List<Group> groups)
        {
            document = XDocument.Load(Path);
            XElement root = document.Root;
            root.RemoveAll();

            try
            {
                foreach (var group in groups)
                {
                    //Группа
                    XElement groupElem = new XElement("Group");
                    XAttribute groupAttribute = new XAttribute("name", group.Name);
                    groupElem.Add(groupAttribute);

                    foreach(var student in group.Students)
                    {
                        //Участник группы
                        XElement studentElem = new XElement("Student");
                        XElement secondNameElem = new XElement("SecondName");
                        XElement nameElem = new XElement("Name");

                        secondNameElem.Add(student.Split(' ').First());
                        nameElem.Add(student.Split(' ').Last());
                        studentElem.Add(secondNameElem, nameElem);

                        groupElem.Add(studentElem);
                    }
                    root.Add(groupElem);
                }
            }
            catch (Exception) 
            {
                return false;
            }

            document.Save(Path);
            return true;
        }

        public List<string> GetGroupsName()
        {
            List<string> groupsNames = new List<string>();            
            XElement root = document.Root;

            //Ищем имена
            foreach (XElement groupElem in root.Elements().ToList())
                groupsNames.Add(groupElem.Attribute("name").Value);

            return groupsNames;
        }

        public Group GetGroup(string groupName)
        {
            // Создаем новую группу
            Group group = new Group()
            {
                Name = groupName
            };
            // Читаем все группы из path
            XmlDocument document = new XmlDocument();

            document.Load(Path);
            XmlElement root = document.DocumentElement;

            foreach (XmlNode groupNode in root)
            {
                foreach(XmlAttribute attrName in groupNode.Attributes)
                {
                    if(attrName.Value == groupName)
                    {
                        foreach (XmlNode studentNode in groupNode)
                        {
                            // Собираем все элементы узла Group в коллекцию Group.List<string>
                            string student = ""; 
                            foreach (XmlNode infoNode in studentNode.ChildNodes)
                            {
                    
                                if (infoNode.Name == "SecondName")                    
                                    student = infoNode.InnerText + " ";
                    
                                if (infoNode.Name == "Name")                    
                                    student += infoNode.InnerText;
                                
                            }
                            group.Students.Add(student);
                        }
                    }
                }
            }
            group.Students.Sort();
            return group;
        }
    }
}
