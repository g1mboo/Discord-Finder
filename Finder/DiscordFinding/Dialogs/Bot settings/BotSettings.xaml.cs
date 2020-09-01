using DiscordFinding.Controls;
using DiscordFinding.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DiscordFinding.Dialogs.Bot_settings
{
    /// <summary>
    /// Логика взаимодействия для BotSettings.xaml
    /// </summary>
    public partial class BotSettings : Window, IMessage
    {   
        private StreamBotXmlConfig botXmlConfig;
        private bool _dialogResult = false;

        public BotSettings()
        {            
            InitializeComponent();             
        }        

        private void Window_Loaded(object sender, RoutedEventArgs e)        {
            
            botXmlConfig = new StreamBotXmlConfig(@"config.xml");

            //Inizialize config
            ModifiedConfig.CommandPrefix = botXmlConfig.GetPartConfig("CommandPrefix");
            ModifiedConfig.MessageTrigger = botXmlConfig.GetPartConfig("MessageTrigger");
            ModifiedConfig.StartPhrase = botXmlConfig.GetPartConfig("StartPhrase");
            ModifiedConfig.PromptsParam = botXmlConfig.GetPartConfig("PromptsParam") == "true" ? true : false;

            InitializeFields();
        }

        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = _dialogResult;
        }      

        private void InitializeFields()
        {
            phraseField.Text = ModifiedConfig.StartPhrase;
            messageTriggerField.Text = ModifiedConfig.MessageTrigger;
            commandPrefixField.Text = ModifiedConfig.CommandPrefix;
            promptsCheckBox.IsChecked = ModifiedConfig.PromptsParam;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Validation phraseField
                if (phraseField.Text == string.Empty)                
                    throw new Exception(BasicTextMessages.IfEmptyField);

                //Validation messageTriggerField
                if (messageTriggerField.Text == string.Empty)                
                    throw new Exception(BasicTextMessages.IfEmptyField);

                //Validation commandPrefixField
                if (commandPrefixField.Text == string.Empty)                
                    throw new Exception(BasicTextMessages.IfEmptyField);

                                
                ModifiedConfig.StartPhrase = phraseField.Text;
                ModifiedConfig.MessageTrigger = messageTriggerField.Text;
                ModifiedConfig.CommandPrefix = commandPrefixField.Text;
                ModifiedConfig.PromptsParam = (bool)promptsCheckBox.IsChecked;

                //Saved
                ModifiedConfig.Save(botXmlConfig);
                _dialogResult = true;

                //Successful
                WriteMessage(BasicTextMessages.IfSuccessfulShort, Colors.Green);
            }
            catch (Exception ex) 
            {
                WriteMessage(ex.Message, Colors.Red);
            }
        }

        private void ClearMessageField(object sender, TextChangedEventArgs e)
        {
            WriteMessage(string.Empty, Colors.White);
        }

        public void WriteMessage(string message, Color color)
        {
            validationField.Foreground = new SolidColorBrush(color);
            validationField.Text = message;
        }

        private struct ModifiedConfig
        {
            public static string StartPhrase { get; set; }
            public static string MessageTrigger { get; set; }
            public static string CommandPrefix { get; set; }
            public static bool PromptsParam { get; set; }

            public static void Save(StreamBotXmlConfig config)
            {
                config.SetPartConfig(nameof(StartPhrase), StartPhrase);
                config.SetPartConfig(nameof(MessageTrigger), MessageTrigger);
                config.SetPartConfig(nameof(CommandPrefix), CommandPrefix);
                config.SetPartConfig(nameof(PromptsParam), PromptsParam ? "true" : "false");
            }
        }

    }
}
