using DiscordFinding.Controls;
using DiscordFinding.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiscordFinding.Dialogs.Group_settings.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для Group.xaml
    /// </summary>
    public partial class GroupWindow : Window, Model.IMessage
    {  
        private bool _dialogResult = false;

        private List<Group> _groups;
        private object _editableObjectAdditional;
        public EWindowMetod _method;

        public GroupWindow(List<Group> groups, string concreteGroupName)
        {
            InitializeComponent();
            _method = EWindowMetod.Edit;
            _groups = groups;
            _editableObjectAdditional = concreteGroupName;
        }

        public GroupWindow(List<Group> groups)
        {
            InitializeComponent();
            _method = EWindowMetod.Add;
            _groups = groups;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch(_method)
            {
                case EWindowMetod.Edit:
                    ConfigurateWindowToEdit();
                    break;
                case EWindowMetod.Add:
                    ConfigurateWindowToAdd();
                    break;
            }            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = _dialogResult;
        }

        private void ConfigurateWindowToEdit()
        {
            Title = "Изменить название группы";            
            groupName.Text = (string)_editableObjectAdditional;
            editableButton.Content = "Изменить";
            editableButton.Click += EditButton_Click;
        }

        private void ConfigurateWindowToAdd()
        {
            Title = "Добавить группу";
            editableButton.Content = "Добавить";
            editableButton.Click += AddButton_Click;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Group group = default; 
            try
            {
                group = _groups.Where(g => g.Name == _editableObjectAdditional as string).First();                
                //Проверка на корректность введеных данных
                if (string.IsNullOrWhiteSpace(groupName.Text))
                    throw new ArgumentNullException();
                if (_groups.Where(g => g.Name == groupName.Text).First() != null)
                    throw new ArgumentOutOfRangeException();               
            }
            catch (ArgumentNullException)
            {
                WriteMessage(BasicTextMessages.IfEmptyField, Colors.Red);
            }
            catch (ArgumentOutOfRangeException)
            {
                WriteMessage(BasicTextMessages.IfNonUnicField, Colors.Red);
            }
            catch (InvalidOperationException)
            {
                group.Name = groupName.Text;
                WriteMessage(BasicTextMessages.IfNameChange, Colors.Green);
                _dialogResult = true;               
            }
            catch (Exception) { }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                //Проверка на корректность введеных данных
                if (string.IsNullOrWhiteSpace(groupName.Text))
                    throw new ArgumentNullException();
                if (_groups.Where(group => group.Name == groupName.Text).First() != null)
                    throw new ArgumentOutOfRangeException();
            }
            catch (ArgumentNullException)
            {
                WriteMessage(BasicTextMessages.IfEmptyField, Colors.Red);
            }
            catch (ArgumentOutOfRangeException)
            {
                WriteMessage(BasicTextMessages.IfNonUnicField, Colors.Red);
            }
            catch (InvalidOperationException)
            {
                _groups.Add(new Group(groupName.Text));
                WriteMessage(BasicTextMessages.IfSuccessfulShort, Colors.Green);
                _dialogResult = true;
            }
            catch (Exception) { }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

        public void WriteMessage(string message, Color color)
        {
            SolidColorBrush colorBrush = new SolidColorBrush(color);

            validationField.Text = message;
            validationField.Foreground = colorBrush;

            //Выключаем сообщение через 10 сек
            ColorAnimationUsingKeyFrames colorAnimation = new ColorAnimationUsingKeyFrames();
            colorAnimation.Duration = TimeSpan.FromSeconds(10);
            colorAnimation.KeyFrames.Add(
               new LinearColorKeyFrame(
                   color,
                   KeyTime.FromTimeSpan(TimeSpan.FromSeconds(9.5)))
               );

            colorAnimation.KeyFrames.Add(
               new LinearColorKeyFrame(
                   Colors.Transparent,
                   KeyTime.FromTimeSpan(TimeSpan.FromSeconds(10)))
               );

            colorBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
        }

    }
}
