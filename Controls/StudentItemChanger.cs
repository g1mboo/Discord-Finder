using DiscordFinding.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DiscordFinding.Controls
{
    public static class ChangedStudentItem
    {
        private static double animationTime = 0.1;
        private static double presenceIndicatorWidth = 20;
        private static double absenceIndicatorWidth = 5;
        private static Color presenceIndicatorColor = (Color)Application.Current.Resources["IndicatorOnColor"];
        private static Color absenceIndicatorColor = (Color)Application.Current.Resources["SystemBaseMediumColor"];
        private static Color presenceTextColor = (Color)Application.Current.Resources["SystemBaseHighColor"];
        private static Color absenceTextColor = (Color)Application.Current.Resources["SystemBaseMediumColor"];
        private static FontWeight presenceTextWeight = FontWeights.DemiBold;
        private static FontWeight absenceTextWeight = FontWeights.Normal;

        public static void IncludeItem(Dictionary<string, Rectangle> indicators, Dictionary<string, TextBlock> text, string studentName)
        {       
            if (indicators[studentName].ActualWidth != presenceIndicatorWidth)
            {
                ChangeIndicator(indicators, studentName, absenceIndicatorColor, presenceIndicatorColor, presenceIndicatorWidth);
                ChangeName(text, studentName, absenceTextColor, presenceTextColor, presenceTextWeight);
            }     
        }
        public static void RevertAllItemsDefault(Dictionary<string, Rectangle> indicators, Dictionary<string, TextBlock> text, Group students)
        {
            for (int i = 0; i < indicators.Count; i++)
            {
                string studentName = students.Students[i].ToString();
                if (indicators[studentName].ActualWidth == presenceIndicatorWidth)
                {
                    ChangeIndicator(indicators, studentName, presenceIndicatorColor, absenceIndicatorColor, absenceIndicatorWidth);
                    ChangeName(text, studentName, presenceIndicatorColor, absenceTextColor, absenceTextWeight);
                }
            }
        }
        private static void ChangeIndicator(Dictionary<string, Rectangle> indicators, string studentName, Color colorFrom, Color colorTo, double widthTo)
        {
            DoubleAnimation widthanimation = new DoubleAnimation(indicators[studentName].ActualWidth, widthTo, TimeSpan.FromSeconds(animationTime));
            ColorAnimation colorAnimation = new ColorAnimation(colorFrom, colorTo, TimeSpan.FromSeconds(animationTime));

            indicators[studentName].BeginAnimation(FrameworkElement.WidthProperty, widthanimation);
            indicators[studentName].Fill.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);           
        }
        private static void ChangeName(Dictionary<string, TextBlock> text, string studentName, Color colorFrom, Color colorTo, FontWeight fontWeight)
        {
            ColorAnimation colorAnimation = new ColorAnimation(colorFrom, colorTo, TimeSpan.FromSeconds(animationTime));

            text[studentName].Foreground.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
            text[studentName].FontWeight = fontWeight;
        }
    }
}
