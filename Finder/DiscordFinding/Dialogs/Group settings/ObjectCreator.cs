using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DiscordFinding.Dialogs.Group_settings
{
    public class ObjectCreator
    {
        public Grid CreateStudentGrid(object tag)
        {
            Grid grid = new Grid();
            for (int i = 0; i < 2; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.Tag = tag;
            return grid;
        }

        public Button CreateStudentEditButton(object tag)
        {
            Button button = new Button
            {
                Style = (Style)Application.Current.Resources["ButtonRoundAccentRevealStyle"],
                Content = "Изменить участника",
                Height = 25,
                Width = 300,
                Tag = tag
            };
            return button;
        }

        public CheckBox CreateStudentCheckBox(object tag)
        {
            CheckBox checkBox = new CheckBox
            {
                Style = (Style)Application.Current.Resources["StudentCheckBox"],
                Content = (string)tag,
                Tag = tag
            };
            return checkBox;
        }
    }
}
