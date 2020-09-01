using DiscordFinding.Controls;
using DiscordFinding.Dialogs.Group_settings.Dialogs;
using DiscordFinding.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace DiscordFinding.Dialogs.Group_settings
{
    /// <summary>
    /// Логика взаимодействия для GroupSettings.xaml
    /// </summary>
    public partial class GroupSettings : Window , IMessage
    {
        private readonly List<Group> _oldGroups;
        private readonly StreamGroup _streamGroup;
        private bool                 _firstWindowStart = true;
        private bool                 _dialogResult = false;
        private List<Group>          _groups;
        private Group                _groupNow;
        private string               _lastViewGroupName;        

        public GroupSettings(string lastViewGroupName)
        {           
            InitializeComponent();
            _streamGroup = new StreamGroup();           
            _lastViewGroupName = lastViewGroupName;
            _oldGroups = _streamGroup.GetGroups();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Добавляем в словарь все существующие группы из файла
            _groups = _streamGroup.GetGroups();

            //Инициализируем все группы в groupNameComboBox
            InitializeGroupComboBoxContent();

            try
            {
                //Пытаемя включить группу
                if (_groups.Where(group => group.Name == _lastViewGroupName).Count() != 0)
                    _groupNow = _groups
                        .Where(group => group.Name == _lastViewGroupName)
                        .First();
                else
                    _groupNow = _groups.First();
                OnActualGroup();
            }
            catch (InvalidOperationException)
            {
                WriteMessage(BasicTextMessages.IfEmptyList, Colors.Red);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //Инициализируем контент участников
            InitializeStudentsEditPanelContent();

            //Объявляем о том что окно было инициализировано
            _firstWindowStart = false;
        }

        private void InitializeGroupComboBoxContent()
        {   
            groupsNameComboBox.Items.Clear();

            foreach (Group group in _groups)
            {
                ComboBoxItem item = new ComboBoxItem();
                Separator separator = new Separator();
                item.Style = (Style)Application.Current.Resources["GroupsItem"];
                item.Content = group.Name;                

                groupsNameComboBox.Items.Add(separator);
                groupsNameComboBox.Items.Add(item);
            }

            Separator separatorEnd = new Separator();
            groupsNameComboBox.Items.Add(separatorEnd);
        }

        private void InitializeStudentsEditPanelContent()
        {
            ObjectCreator creator = new ObjectCreator();
            studentsEditPanel.Children.Clear();

            for (int i = 0; i < _groupNow?.Students.Count; i++)
            {
                Grid grid = creator.CreateStudentGrid(_groupNow.Students[i]);
                Button button = creator.CreateStudentEditButton(_groupNow.Students[i]);
                button.Click += EditStudentButton_Click;
                CheckBox checkBox = creator.CreateStudentCheckBox(_groupNow.Students[i]);

                Grid.SetColumn(checkBox, 0);
                Grid.SetColumn(button, 1);

                grid.Children.Add(checkBox);
                grid.Children.Add(button);

                studentsEditPanel.Children.Add(grid);
            }            
        }

        private void OnActualGroup()
        {
            foreach(var item in groupsNameComboBox.Items)
            {
                //Включаем последнюю увиденную группу
                try
                {
                    //Изменяем имя последней увиденной группы на актуальное имя
                    if (!_firstWindowStart)
                        _lastViewGroupName = _groupNow.Name;
                    //Выбираем эту группу
                    if(((ComboBoxItem)item).Content as string == _lastViewGroupName)
                        ((ComboBoxItem)item).IsSelected = true;            
                }
                catch(InvalidCastException)
                { 
                    continue;
                }
            }
        }

        private void GroupNameComboBox_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (((ComboBox)sender).Text != _groupNow?.Name)
                {
                    _groupNow = _groups
                        .Where(group => group.Name == ((ComboBox)sender).Text)
                        .First();

                    InitializeStudentsEditPanelContent();
                }

                if(!_groups.Any())
                    throw new InvalidOperationException();
            }
            catch (InvalidOperationException)
            {
                WriteMessage("Список пока пуст, но вы можете добавить новую группу", Colors.Orange);
            }
            catch (Exception) { }
        }

        #region Buttons_click event

        private void EditGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (_groupNow != null && _groups.Any())
            {
                GroupWindow window = new GroupWindow(_groups, groupsNameComboBox.Text);
                if ((bool)window.ShowDialog())
                {
                    InitializeGroupComboBoxContent();
                    OnActualGroup();
                }
            }
            else WriteMessage(BasicTextMessages.IfEmptyItemTryChange, Colors.Red);
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            GroupWindow window = new GroupWindow(_groups);

            if ((bool)window.ShowDialog())
            {
                InitializeGroupComboBoxContent();

                if (_groupNow == null)
                    _groupNow = _groups[0];

                OnActualGroup();                
            }
        }

        private void RemoveGroupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = default;

                //Проверяем есть ли группы впринципе
                if (_groups.Any())
                {
                    result = MessageBox.Show(
                    $"Вы уверенны что хотите удалить группу: {groupsNameComboBox.Text}?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _groups.Remove(_groupNow);

                        InitializeGroupComboBoxContent();

                        this._groupNow = _groups[0];                            

                        OnActualGroup();
                        InitializeStudentsEditPanelContent();
                    }
                }
                else throw new Exception(BasicTextMessages.IfEmptyList);
            }
            catch (ArgumentOutOfRangeException)
            {
                this._groupNow = null;
                studentsEditPanel.Children.Clear();
                WriteMessage(BasicTextMessages.IfEmptyList, Colors.Red);
            }
            catch(Exception ex)
            {
                WriteMessage(ex.Message, Colors.Red);
            }

        }

        private void AddStudentsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_groupNow != null)
            {
                UserWindow window = new UserWindow(_groupNow);
                if ((bool)window.ShowDialog())
                {
                    InitializeStudentsEditPanelContent();
                }
            }
            else WriteMessage(BasicTextMessages.IfEmptyItemTryModificate, Colors.Red);
        }

        private void EditStudentButton_Click(object sender, RoutedEventArgs e)
        {
            UserWindow window = new UserWindow(_groupNow, ((Button)sender).Tag as string);

            if ((bool)window.ShowDialog())            
                InitializeStudentsEditPanelContent();            
        }

        private void RemoveStudentsButton_Click(object sender, RoutedEventArgs e)
        {
            string message = "Вы уверены что хотите удалить следующих участников?\r\n";

            List<string> studentsToRemove = new List<string>();
            List<Grid> studentsCollection = studentsEditPanel.Children.OfType<Grid>().ToList();
            List<CheckBox> studentsCheckBoxCollection = new List<CheckBox>();

            foreach (var item in studentsCollection)
                studentsCheckBoxCollection
                    .Add(item.Children.OfType<CheckBox>()
                    .First());

            try
            {
                bool _hasItemsToDelete = false;
                foreach (var item in studentsCheckBoxCollection)
                    if ((bool)item.IsChecked)
                    {
                        studentsToRemove.Add(item.Tag as string);
                        message += item.Tag as string + Environment.NewLine;
                        _hasItemsToDelete = true;
                    }

                if (_hasItemsToDelete)
                {
                    MessageBoxResult result = MessageBox
                        .Show(message, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        foreach (var item in studentsToRemove)
                        {
                            _groupNow.Students.Remove(item);

                            studentsEditPanel.Children
                                .Remove(studentsCollection
                                .Where(listItem => listItem.Tag as string == item)
                                .First());
                        }
                    }
                }
                else throw new InvalidOperationException();

            }
            catch (InvalidOperationException) 
            {
                WriteMessage(BasicTextMessages.IfNoItemIsSelected, Colors.Red);
            }
            catch (Exception) { }

        }

        private void ResetChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if(ChangesRequiredToBeReset())
            {
                Window_Loaded(null, null);
                WriteMessage(BasicTextMessages.IfChangesReset, Colors.Green);
            }
            else WriteMessage(BasicTextMessages.IfNoChangesRequired, Colors.Orange);
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if(ChangesRequiredToBeReset())
                if (_streamGroup.SetGroups(_groups))
                {
                    _dialogResult = true;
                    WriteMessage(BasicTextMessages.IfSuccessfulUltraShort, Colors.Green);
                }
                else WriteMessage(BasicTextMessages.IfSuccessfulUltraShort, Colors.Red);
            else WriteMessage(BasicTextMessages.IfNoChangesRequired, Colors.Orange);

        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e) => Close();

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = _dialogResult;
        }

        public bool ChangesRequiredToBeReset()
        {
            if (_oldGroups.Count == _groups.Count)
                for (int i = 0; i < _oldGroups.Count; i++)
                {
                    if(_oldGroups[i].Name == _groups[i].Name & _oldGroups[i].Students.Count == _groups[i].Students.Count)
                    {
                        for (int j = 0; j < _oldGroups[i].Students.Count; j++)
                        {
                            if(_oldGroups[i].Students[j].ToString() != _groups[i].Students[j].ToString())
                                return true;                            
                        }                        
                    }
                    else return true;
                }
            else return true;

            return false;
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
