using BusinessLogicLayer.Models;
using BusinessLogicLayer.Services;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Chess.Views
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        public int GameTime { get; set; }

        private TimeSpan wStartTime;
        private TimeSpan bStartTime;

        private DispatcherTimer whiteTimer;
        private DispatcherTimer blackTimer;

        public GamePage()
        {
            InitializeComponent();
            SetTimers();
            ChessGame.BlackKilled += OnBlackKilled;
            ChessGame.WhiteKilled += OnWhiteKilled;
            ChessGame.FigureMoved += ChessGame_FigureMoved;
            ChessGame.GameStarting += (s, e) => {
                whiteKilledWrap.Children.Clear();
                blackKilledWrap.Children.Clear();
                whiteLogList.Children.Clear();
                blackLogList.Children.Clear();
                if (s != null)
                {
                    var gm = s as GameModel;
                    SetAmoutOfTime(ChessGame.WhiteTime, ChessGame.BlackTime);
                    foreach (var f in gm.WhiteKilled)
                        OnWhiteKilled(FigureDataModel.ToFigure(f), null);
                    foreach (var f in gm.BlackKilled)
                        OnBlackKilled(FigureDataModel.ToFigure(f), null);
                    bool isWhite = true;
                    foreach (var log in ChessGame.Logs)
                    {
                        StackPanel stackPanel = new StackPanel()
                        {
                            Orientation = Orientation.Horizontal,
                        };
                        Image image = new Image()
                        {
                            Height = 30,
                            Width = 30,
                            Margin = new Thickness(5),
                            Source = new BitmapImage(new Uri($"/Images/{log.Item1}.png", UriKind.Relative)),
                        };
                        TextBlock textBlock = new TextBlock()
                        {
                            Text = log.Item2,
                            FontSize = 18,
                            Foreground = Brushes.White
                        };
                        stackPanel.Children.Add(image);
                        stackPanel.Children.Add(textBlock);
                        if (isWhite)
                            whiteLogList.Children.Add(stackPanel);
                        else blackLogList.Children.Add(stackPanel);
                        isWhite = !isWhite;
                    }
                    logScrollViewer.PageDown();
                    if (ChessGame.WhiteTurn)
                        whiteTimer.Start();
                    else blackTimer.Start();
                }
                else {
                    SetAmoutOfTime(new TimeSpan(0, GameTime, 0), new TimeSpan(0, GameTime, 0));
                    whiteTimer.Start();
                }
            };
            ChessGame.GameEnding += (s, e) =>
            {
                whiteTimer.Stop();
                blackTimer.Stop();
            };
        }

        private void SetAmoutOfTime(TimeSpan timeWhite, TimeSpan timeBlack)
        {
            wTimeTextBlock.Text = $"{timeWhite.Minutes.ToString("D2")}:{timeWhite.Seconds.ToString("D2")}";
            bTimeTextBlock.Text = $"{timeBlack.Minutes.ToString("D2")}:{timeBlack.Seconds.ToString("D2")}";
            wStartTime = timeWhite;
            bStartTime = timeBlack;

            ChessGame.WhiteTime = timeWhite;
            ChessGame.BlackTime = timeBlack;
        }

        private void SetTimers()
        {
            whiteTimer = new DispatcherTimer(DispatcherPriority.Normal);
            whiteTimer.Interval = TimeSpan.FromSeconds(1);
            whiteTimer.Tick += (s, e) =>
            {
                wStartTime = wStartTime.Subtract(TimeSpan.FromSeconds(1));
                wTimeTextBlock.Text = $"{wStartTime.Minutes.ToString("D2")}:{wStartTime.Seconds.ToString("D2")}";
                if (wStartTime.TotalSeconds <= 0)
                {
                    ChessGame.EndGame(GameState.Black);
                }
            };

            blackTimer = new DispatcherTimer(DispatcherPriority.Normal);
            blackTimer.Interval = TimeSpan.FromSeconds(1);
            blackTimer.Tick += (s, e) =>
            {
                bStartTime = bStartTime.Subtract(TimeSpan.FromSeconds(1));
                bTimeTextBlock.Text = $"{bStartTime.Minutes.ToString("D2")}:{bStartTime.Seconds.ToString("D2")}";
                if (bStartTime.TotalSeconds <= 0)
                {
                    ChessGame.EndGame(GameState.White);
                }
            };
        }

        private void ChessGame_FigureMoved(object sender, EventArgs e)
        {
            IFigure figure = (IFigure)sender;
            StackPanel stackPanel = new StackPanel() {
                Orientation = Orientation.Horizontal,
            };
            Image image = new Image() {
                Height = 30, Width = 30, Margin = new Thickness(5),
                Source = new BitmapImage(new Uri($"/Images/{ChessConverter.ConvertFigureToSource(figure)}.png", UriKind.Relative)),
            };
            TextBlock textBlock = new TextBlock()
            {
                Text = $"{(char)('a' + figure.Position.X)}{8 - figure.Position.Y}",
                FontSize = 18,
                Foreground = Brushes.White
            };
            stackPanel.Children.Add(image);
            stackPanel.Children.Add(textBlock);
            if (figure.IsWhite)
                whiteLogList.Children.Add(stackPanel);
            else blackLogList.Children.Add(stackPanel);
            logScrollViewer.PageDown();

            if (figure.IsWhite)
            {
                whiteTimer.Stop();
                blackTimer.Start();
            }
            else
            {
                whiteTimer.Start();
                blackTimer.Stop();
            }
            ChessGame.WhiteTime = wStartTime;
            ChessGame.BlackTime = bStartTime;
            ChessGame.Logs.Add(Tuple.Create(
                ChessConverter.ConvertFigureToSource(figure),
                $"{(char)('a' + figure.Position.X)}{8 - figure.Position.Y}"));
        }

        private void OnBlackKilled(object sender, EventArgs e)
        {
            blackKilledWrap.Children.Add(GetKilledFigure(sender));
        }

        private void OnWhiteKilled(object sender, EventArgs e)
        {
            whiteKilledWrap.Children.Add(GetKilledFigure(sender));
        }

        private Image GetKilledFigure(object sender)
        {
            string path = ChessConverter.ConvertFigureToSource(sender as IFigure);
            Image killedFigure = new Image()
            {
                Height = 25,
                Width = 25,
                Source = new BitmapImage(new Uri($"/Images/{path}.png", UriKind.Relative))
            };
            return killedFigure;
        }
    }
}
