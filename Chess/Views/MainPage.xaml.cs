using BusinessLogicLayer.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public int GameTime { get; set; }

        public MainPage()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Properties.Settings.Default.Culture);
            GameTime = Properties.Settings.Default.Time;
            timeTextBlock.Text = $"{GameTime}";
            UpdateLocalization();

            if (ChessGame.IsSavedGame)
            {
                (continueButton.Parent as Border).Visibility = Visibility.Visible;
            }
        }

        public void UpdateLocalization()
        {
            (playButton.Content as TextBlock).Text = Strings.Play;
            (continueButton.Content as TextBlock).Text = Strings.Continue;
            rulesTextBlock.Text = Strings.ReadRules;
            minutesTextBlock.Text = ' ' + Strings.Minutes;
        }

        private void upNum_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GameTime = Math.Min(GameTime + 5, 30);
            ShowTimeAndSave();
        }

        private void downNum_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GameTime = Math.Max(GameTime - 5, 5);
            ShowTimeAndSave();
        }

        private void ShowTimeAndSave()
        {
            timeTextBlock.Text = $"{GameTime}";
            Properties.Settings.Default.Time = (byte)GameTime;
            Properties.Settings.Default.Save();
        }
    }
}
