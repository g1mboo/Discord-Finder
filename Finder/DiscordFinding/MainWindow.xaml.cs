using DiscordFinding.Controls;
using DiscordFinding.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Windows.Threading;
using System.Configuration;
using System.Runtime.CompilerServices;
using MaterialDesignThemes.Wpf;
using DiscordFinding.Dialogs.Bot_settings;
using DiscordFinding.Dialogs.Group_settings;
using DiscordFinding.Dialogs.About_program;
using System.Windows.Media.Effects;

namespace DiscordFinding
{    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private List<Group>     _groups;
        private Group           _groupNow;
        private string          _lastViewGroupName;

        private DispatcherTimer timer;
        private Process         discordBot;

        private Dictionary<string, Rectangle> Indicators { get; set; }
        private Dictionary<string, TextBlock> Names { get; set; }

        protected StreamGroup _streamGroup;

        public MainWindow()
        { 
            InitializeComponent();
            StateChanged += MainWindowStateChangeRaised;                     
        }        

        #region Standart Title Settings
        // Can execute
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Minimize
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        // Maximize
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);

            HeightDoubleAnimationStart<Button>(CollapseMenu, CollapseMenu.ActualHeight, 0, 0);

            if (!menuOpen)
            {                
                WindowTitle.Text = "Discord Finding";
                WindowTitle.FontWeight = FontWeights.Normal;
                WidthDoubleAnimationStart<Grid>(MenuPanel, MenuPanel.ActualWidth, MenuVisibleWidth, 0);
                HeightDoubleAnimationStart<TextBlock>(BotMenuBlock, BotMenuBlock.ActualHeight, visibleTitleHeight, 0);
                HeightDoubleAnimationStart<TextBlock>(AppMenuBlock, AppMenuBlock.ActualHeight, visibleTitleHeight, 0);
                HeightDoubleAnimationStart<Separator>(BotSeparator, BotSeparator.ActualHeight, 0, 0);
                HeightDoubleAnimationStart<Separator>(AppSeparator, AppSeparator.ActualHeight, 0, 0);
            }                        
        }

        // Restore
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);

            HeightDoubleAnimationStart<Button>(CollapseMenu, CollapseMenu.ActualHeight, CollapseButtonHeight, 0);

            if (!menuOpen)
            {
                WindowTitle.Text = "DF";
                WindowTitle.FontWeight = FontWeights.DemiBold;
                WidthDoubleAnimationStart<Grid>(MenuPanel, MenuPanel.ActualWidth, MenuHideWidth, 0);
                HeightDoubleAnimationStart<TextBlock>(BotMenuBlock, BotMenuBlock.ActualHeight, 0, 0);
                HeightDoubleAnimationStart<TextBlock>(AppMenuBlock, AppMenuBlock.ActualHeight, 0, 0);
                HeightDoubleAnimationStart<Separator>(BotSeparator, BotSeparator.ActualHeight, visibleSeparatorHeight, 0);
                HeightDoubleAnimationStart<Separator>(AppSeparator, AppSeparator.ActualHeight, visibleSeparatorHeight, 0);
            }
                
        }

        // Close
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        // State change
        private void MainWindowStateChangeRaised(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {                
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {                
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }
        }

        //DragMove Title
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        #endregion

        #region Window loading/closing actions
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _lastViewGroupName = ConfigurationManager.AppSettings["lastGroup"];
            StreamGroup.Path = @"Content\Groups.xml";
            _streamGroup = new StreamGroup();            

            //Инициализируем контент
            InitializeDiscordBot();
            InitializeGroupList();
            InitializeContent();
            InitializeTimer();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                File.Delete(@"students.log");
                discordBot.Kill();
            }
            catch { }
            finally
            {
                SaveConfigurations();
            }
        }        
        #endregion

        #region Programm Content
        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(EnableDisableIndicators);
            timer.Interval = new TimeSpan(0,0,1);
            timer.Start();
        }

        private void InitializeDiscordBot()
        {            
            try
            {
                discordBot = new Process();
                discordBot.StartInfo.UseShellExecute = false;            
                discordBot.StartInfo.FileName = @"DiscordBot\DiscordFindingBot.exe";
                discordBot.StartInfo.CreateNoWindow = true;
                discordBot.Start();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }
        private void InitializeGroupList()
        {            
            try
            {
                _groups = _streamGroup.GetGroups();

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

                ComboBoxItem toOnItem;
                if ((toOnItem = groupsNameComboBox.Items.OfType<ComboBoxItem>().ToList().Where(ComboBoxItem => ComboBoxItem.Content as string == _lastViewGroupName).FirstOrDefault()) != null)
                    toOnItem.IsSelected = true;
                else groupsNameComboBox.Items.OfType<ComboBoxItem>().ToList().First();

                Separator separatorEnd = new Separator();
                groupsNameComboBox.Items.Add(separatorEnd);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void InitializeContent()
        {
            Indicators = new Dictionary<string, Rectangle>();
            Names = new Dictionary<string, TextBlock>();

            try
            {
                _groupNow = _groups
                    .Where(group => group.Name == groupsNameComboBox.Text)
                    .FirstOrDefault();

                studentsList.Children.Clear();            

                for (int i = 0; i < _groupNow?.Students.Count; i++)
                {
                    Grid studentItem = new Grid();
                    Rectangle studentIndicator = new Rectangle();
                    TextBlock studentFullName = new TextBlock();
                    Button studentButton = new Button();

                                    
                    studentItem.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });                    
                    studentItem.ColumnDefinitions.Add(new ColumnDefinition());                    
                    studentItem.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });                    

                    studentItem.MinHeight = 30;
                    studentIndicator.Style = (Style)Application.Current.Resources["IndicatorDefault"];
                    Grid.SetColumn(studentIndicator, 0);

                    studentFullName.Text = _groupNow.Students[i];
                    studentFullName.Style = (Style)Application.Current.Resources["StudentItemTextBlockDefault"];
                    Grid.SetColumn(studentFullName, 1);

                    studentButton.Style = (Style)Application.Current.Resources["DiamondsButton"];
                    Grid.SetColumn(studentButton, 2);
                    studentButton.Click += DiamondsButton_LeftClick;
                    studentButton.PreviewMouseRightButtonDown += DiamondsButton_RightClick;
                    Border border = new Border
                    {
                        Background = (SolidColorBrush)Application.Current.Resources["SystemBaseLowColorBrush"],
                        CornerRadius = new CornerRadius(12),
                        Padding = new Thickness(6, 5, 6, 5)
                    };
                    StackPanel panel = new StackPanel 
                    { 
                        Orientation = Orientation.Horizontal 
                    };
                    for(int j = 0; j < 3; j++)
                    {
                        PackIcon packIcon = new PackIcon
                        {
                            VerticalAlignment = VerticalAlignment.Center,
                            Kind = PackIconKind.DiamondStone,
                            Foreground = new SolidColorBrush(Colors.DeepSkyBlue),
                            Opacity = 0.2,
                            Width = 18,
                            Height = 18,
                            Margin = new Thickness(3, 0, 3, 0)
                        };
                        panel.Children.Add(packIcon);
                    }
                    border.Child = panel;
                    studentButton.Content = border;

                    studentItem.Children.Add(studentIndicator);
                    studentItem.Children.Add(studentFullName);
                    studentItem.Children.Add(studentButton);
                    studentsList.Children.Add(studentItem);
                
                    Indicators.Add(_groupNow.Students[i], studentIndicator);
                    Names.Add(_groupNow.Students[i], studentFullName);
                }
            }
            catch (InvalidOperationException) { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }  

        private void DiamondsButton_LeftClick(object sender, RoutedEventArgs e)
        {    
            Button button = sender as Button;
            List<PackIcon> icons = ((button
                .Content as Border)
                .Child as StackPanel)
                .Children.OfType<PackIcon>()
                .ToList();


            if (icons.All(x => x.Opacity == 1))
            {
                for (int i = icons.Count - 1; i >= 0; i--)
                {
                    if ((icons[i].Foreground as SolidColorBrush).Color == Colors.DeepSkyBlue)
                    {
                        icons[i].Foreground = new SolidColorBrush(Colors.DeepPink);
                        (icons[i].Effect as DropShadowEffect).Color = 
                            (icons[i].Foreground as SolidColorBrush).Color;
                        break;
                    }
                }
            }
            else
            {
                for (int i = icons.Count - 1; i >= 0; i--)
                {
                    if(icons[i].Opacity == 0.2)
                    {
                        DropShadowEffect dropShadowEffect = new DropShadowEffect
                        {
                            Color = (icons[i].Foreground as SolidColorBrush).Color,
                            BlurRadius = 10,
                            Direction = 0,
                            ShadowDepth = 0
                        };
                        icons[i].Effect = dropShadowEffect;
                        icons[i].Opacity = 1;
                        break;
                    }
                }
            }
                        
        }

        private void DiamondsButton_RightClick(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            List<PackIcon> icons = ((button
                .Content as Border)
                .Child as StackPanel)
                .Children.OfType<PackIcon>()
                .ToList();

            if (icons.Where(x => (x.Foreground as SolidColorBrush).Color == Colors.DeepPink).ToList().Count != 0)
            {
                for (int i = 0; i < icons.Count; i++)
                {
                    if ((icons[i].Foreground as SolidColorBrush).Color == Colors.DeepPink)
                    {
                        icons[i].Foreground = new SolidColorBrush(Colors.DeepSkyBlue);
                        (icons[i].Effect as DropShadowEffect).Color =
                            (icons[i].Foreground as SolidColorBrush).Color;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < icons.Count; i++)
                {
                    if (icons[i].Opacity == 1)
                    {
                        icons[i].Opacity = 0.2;
                        break;
                    }
                }
            }


        }

        //Изменяет текущую группу
        private void GroupName_DropDownClosed(object sender, EventArgs e)
        {            
            if(((ComboBox)sender).Text != _groupNow?.Name)
            {
                InitializeContent();
                _lastViewGroupName = groupsNameComboBox.Text;
            }
        }

        // Сравнивает имена пользователей из 'students.log' файла с именами учеников класса Group        
        private void EnableDisableIndicators(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(File.ReadAllText(@"students.log")))
                {
                    using(StreamReader logReader = new StreamReader(@"students.log"))
                    {
                        string student;                    
                        while ((student = logReader.ReadLine()) != null)
                        {
                            try
                            {                            
                                //Включение индикаторов
                                ChangedStudentItem.IncludeItem(Indicators, Names, student);                            
                            }
                            catch { }
                        }                    
                    }
                }
                else throw new FileNotFoundException();
            }            
            catch (FileNotFoundException)
            {
                try
                {
                    //Очистка индикаторов
                    ChangedStudentItem.RevertAllItemsDefault(Indicators, Names, _groupNow);
                }
                catch { }
            }
            catch(Exception) { }
        } 

        private void SaveConfigurations()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection appSettings = (AppSettingsSection)config.GetSection("appSettings");
            appSettings.Settings["lastGroup"].Value = _lastViewGroupName;
            config.Save();
        }

        #endregion

        #region Right Menu

        //Menu Size Properties
        private const double MenuHideWidth = 50;
        private const double MenuVisibleWidth = 300;
        private const double MenuCollapsedWidth = MenuVisibleWidth - MenuHideWidth;
        private const double MenuAnimationTime = 0.12;
        private const double CollapseButtonHeight = 40;

        private bool menuOpen = true;
        
        //Menu Animations
        private void CollapsedMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Button thisObject = sender as Button;
            if (menuOpen)
            {
                if (WindowState != WindowState.Maximized)
                {
                    HideMenuAnimations();
                    menuOpen = false;
                    thisObject.ToolTip = "Разворачивает меню и показывает название кнопок";
                }
            }
            else
            {
                if (WindowState != WindowState.Maximized)
                {
                    ShowMenuAnimations();
                    menuOpen = true;
                    thisObject.ToolTip = "Сворачивает меню и оставляет иконки кнопок";
                }
            }
        }

        //hide
        private readonly double visibleSeparatorHeight = 20; 
        private void HideMenuAnimations()
        {
            //Resize window
            DoubleAnimation windowAnimation = new DoubleAnimation(ActualWidth, ActualWidth - MenuCollapsedWidth, TimeSpan.FromSeconds(MenuAnimationTime));
            MinWidth -= (MenuVisibleWidth - MenuHideWidth);
            BeginAnimation(WidthProperty, windowAnimation);

            //Menu anim
            ReverseMenuCollapseButtonIcon();
            WindowTitle.Text = "DF";
            WindowTitle.FontWeight = FontWeights.DemiBold;
            WidthDoubleAnimationStart<Grid>(MenuPanel, MenuPanel.ActualWidth, MenuHideWidth, MenuAnimationTime); 
            HeightDoubleAnimationStart<TextBlock>(BotMenuBlock, BotMenuBlock.ActualHeight, 0, MenuAnimationTime);
            HeightDoubleAnimationStart<TextBlock>(AppMenuBlock, AppMenuBlock.ActualHeight, 0, MenuAnimationTime);
            HeightDoubleAnimationStart<Separator>(BotSeparator, BotSeparator.ActualHeight, visibleSeparatorHeight, MenuAnimationTime);
            HeightDoubleAnimationStart<Separator>(AppSeparator, AppSeparator.ActualHeight, visibleSeparatorHeight, MenuAnimationTime);            
        }
        //show
        private readonly double visibleTitleHeight = 50; 
        private void ShowMenuAnimations()
        {
            //Resize window
            DoubleAnimation windowAnimation = new DoubleAnimation(ActualWidth, ActualWidth + MenuCollapsedWidth, TimeSpan.FromSeconds(MenuAnimationTime));
            windowAnimation.Completed += delegate (object source, EventArgs args) { MinWidth += (MenuVisibleWidth - MenuHideWidth); };
            BeginAnimation(WidthProperty, windowAnimation);

            //Menu anim
            ReverseMenuCollapseButtonIcon();
            WindowTitle.Text = "Discord Finding";
            WindowTitle.FontWeight = FontWeights.Normal;            
            WidthDoubleAnimationStart<Grid>(MenuPanel, MenuPanel.ActualWidth, MenuVisibleWidth, MenuAnimationTime);
            HeightDoubleAnimationStart<TextBlock>(BotMenuBlock, BotMenuBlock.ActualHeight, visibleTitleHeight, MenuAnimationTime);
            HeightDoubleAnimationStart<TextBlock>(AppMenuBlock, AppMenuBlock.ActualHeight, visibleTitleHeight, MenuAnimationTime);
            HeightDoubleAnimationStart<Separator>(BotSeparator, BotSeparator.ActualHeight, 0, 0);
            HeightDoubleAnimationStart<Separator>(AppSeparator, AppSeparator.ActualHeight, 0, 0);            
        }

        private void ReverseMenuCollapseButtonIcon()
        {
            RotateTransform transform;

            if (menuOpen)
                transform = new RotateTransform(180, MenuCollapseButtonIcon.ActualWidth * 0.5, MenuCollapseButtonIcon.ActualHeight * 0.5);
            else
                transform = new RotateTransform(0, MenuCollapseButtonIcon.ActualWidth * 0.5, MenuCollapseButtonIcon.ActualHeight * 0.5);

            MenuCollapseButtonIcon.RenderTransform = transform;            
        }

        private void HeightDoubleAnimationStart<T>(object obj, double oldValue, double newValue, double duration)
        {
            DoubleAnimation animation = new DoubleAnimation(oldValue, newValue, TimeSpan.FromSeconds(duration));            
            
            if (typeof(T) == typeof(TextBlock))
            {
                TextBlock textBlock = (TextBlock)obj;
                textBlock.BeginAnimation(TextBlock.HeightProperty, animation);
            }
            else if (typeof(T) == typeof(Grid))
            {
                Grid grid = (Grid)obj;
                grid.BeginAnimation(Grid.HeightProperty, animation);
            }
            else if (typeof(T) == typeof(Separator))
            {
                Separator separator = (Separator)obj;                
                separator.BeginAnimation(HeightProperty, animation);
            }
            else if (typeof(T) == typeof(Button))
            {
                Button button = (Button)obj;
                button.BeginAnimation(HeightProperty, animation);
            }
        }

        private void WidthDoubleAnimationStart<T>(object obj, double oldValue, double newValue, double duration)
        {            
            DoubleAnimation animation = new DoubleAnimation(oldValue, newValue, TimeSpan.FromSeconds(duration));
            if (typeof(T) == typeof(TextBlock)) 
            {
                TextBlock textBlock = (TextBlock)obj;
                textBlock.BeginAnimation(TextBlock.WidthProperty, animation);
            }
            else if (typeof(T) == typeof(Grid))
            {
                Grid grid = (Grid)obj;
                grid.BeginAnimation(Grid.WidthProperty, animation);
            }
            else if (typeof(T) == typeof(Separator))
            {
                Separator separator = (Separator)obj;
                separator.BeginAnimation(WidthProperty, animation);
            }
            else if (typeof(T) == typeof(Window))
            {
                Window window = (Window)obj;
                window.BeginAnimation(WidthProperty, animation);
            }
        }        


        //Menu Buttons Click Event        
        private void RestartBot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                discordBot.Kill();
                File.Delete(@"students.log");
            }
            catch { }
            finally
            {
                discordBot.Start();
                ChangedStudentItem.RevertAllItemsDefault(Indicators, Names, _groupNow);
            }
        }
        
        private void BotSettings_Click(object sender, RoutedEventArgs e)
        {
            BotSettings window = new BotSettings();

            if((bool)window.ShowDialog())
            {
                try
                {
                    discordBot.Kill();
                }
                catch { }
                finally
                {
                    discordBot.Start();
                }
            }  
        }

        private void GroupSettings_Click(object sender, RoutedEventArgs e)
        {
            GroupSettings window = new GroupSettings(groupsNameComboBox.Text);
            if ((bool)window.ShowDialog())
            {
                InitializeGroupList();
                InitializeContent();
            }
        }      

        private void AppSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AboutProgram_Click(object sender, RoutedEventArgs e)
        {
            AboutProgram window = new AboutProgram();
            window.ShowDialog();
        }

        #endregion

    }

}
