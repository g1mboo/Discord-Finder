using DiscordFinding.Controls;
using DiscordFinding.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window, Model.IMessage
    {
        private bool _dialogResult = false;
        private Group _group;
        private string _userItem;
        public EWindowMetod _method;
        public UserWindow(Group group)
        {
            InitializeComponent();
            _method = EWindowMetod.Add;
            _group = group;
        }

        public UserWindow(Group group, string userItem)
        {
            InitializeComponent();
            _method = EWindowMetod.Edit;
            _group = group;
            _userItem = userItem;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (_method)
            {
                case EWindowMetod.Edit:
                    ConfigurateWindowToEdit();
                    break;
                case EWindowMetod.Add:
                    ConfigurateWindowToAdd();
                    break;
            }
        }

        private void ConfigurateWindowToEdit()
        {
            Title = "Изменить данные участника";            
            editableButton.Content = "Изменить";
            editableButton.Click += EdittableButton_Click;
            userSecondName.Text = _userItem.Split(' ')[0];
            userName.Text = _userItem.Split(' ')[1];
        }

        private void ConfigurateWindowToAdd()
        {
            Title = "Добавить участника";
            editableButton.Content = "Добавить";
            editableButton.Click += EdittableButton_Click;
        }

        private void EdittableButton_Click(object sender, RoutedEventArgs e)
        {
            string newUserName = $"{userSecondName.Text} {userName.Text}";
            try
            {
                if (!string.IsNullOrWhiteSpace(userName.Text) &
                    !string.IsNullOrWhiteSpace(userSecondName.Text))
                {
                    switch (_method)
                    {
                        case EWindowMetod.Edit:
                            if (_group.Students.All(student => student != newUserName))
                            {
                                _group.Students
                                    .Remove(_group.Students
                                    .Where(student => student == _userItem)
                                    .First());
                                _group.Students.Add(newUserName);
                                _group.Students.Sort();
                                _userItem = newUserName;
                            }
                            else throw new InvalidOperationException();
                            break;
                        case EWindowMetod.Add:
                            if (_group.Students.All(student => student != $"{userSecondName.Text} {userName.Text}"))
                                _group.Students.Add($"{userSecondName.Text} {userName.Text}");
                            else throw new InvalidOperationException();
                            break;

                    }
                    //Если все проверки успешны
                    _dialogResult = true;
                    WriteMessage(BasicTextMessages.IfSuccessfulShort, Colors.Green);
                }
                else
                    WriteMessage(BasicTextMessages.IfEmptyField, Colors.Red);

            }            
            catch (InvalidOperationException)
            {
                WriteMessage(BasicTextMessages.IfUserNonUnicField, Colors.Red);
            }
            catch (Exception) { }
        }        

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = _dialogResult;
        }

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
