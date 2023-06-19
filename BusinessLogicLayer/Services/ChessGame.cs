using BusinessLogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Net;
using DataLogicLayer;
using System.IO;

namespace BusinessLogicLayer.Services
{
    public enum GameState
    {
        White,
        Black,
        Draw
    }

    /*
     TODO
    Доробити continueGame для GamePage
     */

    public static class ChessGame
    {
        private static IFigure[,] board;
        public static bool WhiteTurn { get; private set; }
        public static bool IsSavedGame { get => File.Exists(currentGamePath); }

        public static List<IFigure> WhiteFigures { get; private set; }
        public static List<IFigure> BlackFigures { get; private set; }

        public static List<Tuple<string, string>> Logs { get; private set; }

        private static List<IFigure> killedWhite;
        private static List<IFigure> killedBlack;

        public static Point? ToweringPos = null;
        private static string currentGamePath = "current.xml";

        public static TimeSpan WhiteTime { get; set; }
        public static TimeSpan BlackTime { get; set; }

        public static event EventHandler GameStarting;
        public static event EventHandler BlackKilled;
        public static event EventHandler WhiteKilled;
        public static event EventHandler BoardSetted;
        public static event EventHandler FigureMoved;
        public static event EventHandler GameEnding;

        public static void EndGame(GameState message)
        {
            if (GameEnding != null)
                GameEnding.Invoke(message, null);
            File.Delete(currentGamePath);
        }

        static ChessGame()
        {
            board = new IFigure[8,8];
            WhiteFigures = new List<IFigure>();
            BlackFigures = new List<IFigure>();
            killedBlack = new List<IFigure>();
            killedWhite = new List<IFigure>();
            Logs = new List<Tuple<string, string>>();
        }

        public static void OnFigureMove(IFigure figure)
        {
            if (FigureMoved != null)
                FigureMoved.Invoke(figure, null);
        }

        public static void SetBoard(List<IFigure> figures)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = null;

            figures.ForEach(f => board[f.Position.X, f.Position.Y] = f);

            if(BoardSetted != null)
                BoardSetted.Invoke(figures, null);
        }

        public static bool IsDraw(Point position)
        {
            if (board[position.X, position.Y] == null) return false;
            bool isWhite = !board[position.X, position.Y].IsWhite;
            if(IsCheck(isWhite)) return false;

            if (isWhite) {
                foreach (var f in WhiteFigures)
                {
                    if (GetMoves(f.Position).Count > 0) return false;
                }
            }
            else
            {
                foreach (var f in BlackFigures)
                {
                    if (GetMoves(f.Position).Count > 0) return false;
                }
            }

            return true;
        }

        public static void ChangePawn(Point position, IFigure figure)
        {
            figure.Position = position; 
            figure.IsWhite = board[position.X, position.Y].IsWhite;
            if (figure.IsWhite)
            {
                WhiteFigures.Remove(board[position.X, position.Y]);
                WhiteFigures.Add(figure);
            }
            else
            {
                BlackFigures.Remove(board[position.X, position.Y]);
                BlackFigures.Add(figure);
            }
            board[position.X, position.Y] = figure;
        }

        public static bool CanMove(Point start, Point end) => GetMoves(start).Contains(end) || GetAttacks(start).Contains(end);

        public static void ContinueGame()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    board[i, j] = null;

            killedWhite.Clear();
            killedBlack.Clear();
            WhiteFigures.Clear();
            BlackFigures.Clear();
            Logs.Clear();

            GameModel gameModel = XmlDataBase.Read<GameModel>(currentGamePath);
            FigureDataModel.ToFigure(gameModel.Figures[0]);
            foreach (var f in gameModel.Figures)
                board[f.Position.X, f.Position.Y] = FigureDataModel.ToFigure(f);
            WhiteTurn = gameModel.WhiteTurn;
            killedWhite.AddRange(gameModel.WhiteKilled.Select(f => FigureDataModel.ToFigure(f)));
            killedBlack.AddRange(gameModel.BlackKilled.Select(f => FigureDataModel.ToFigure(f)));
            for (int i = 0; i < gameModel.LogsImg.Count; i++)
                Logs.Add(Tuple.Create(gameModel.LogsImg[i], gameModel.LogsPos[i]));
            WhiteTime = TimeSpan.Parse(gameModel.WhiteTime);
            BlackTime = TimeSpan.Parse(gameModel.BlackTime);

            File.Delete(currentGamePath);

            foreach (var item in board)
            {
                if (item == null) continue;
                if (item.IsWhite)
                    WhiteFigures.Add(item);
                else BlackFigures.Add(item);
            }

            if (GameStarting != null)
                GameStarting(gameModel, null);
        }

        public static void StartGame()
        {
            for(int i = 0; i < 8; i++)
                for(int j = 0; j < 8; j++)
                    board[i, j] = null;

            killedWhite.Clear();
            killedBlack.Clear();
            WhiteFigures.Clear();
            BlackFigures.Clear();
            Logs.Clear();

            WhiteTurn = true;

            // Test
            //board[3, 6] = new Pawn() { Position = new Point(3, 6), IsWhite = true };
            //board[4, 4] = new Knight() { Position = new Point(4, 4), IsWhite = true };
            //board[2, 1] = new Bishop() { Position = new Point(2, 1), IsWhite = true };

            for(int i = 0; i < 8; i++)
            {
                board[i, 6] = new Pawn() { Position = new Point(i, 6), IsWhite = true };
            }
            board[0, 7] = new Tower() { Position = new Point(0, 7), IsWhite = true };
            board[7, 7] = new Tower() { Position = new Point(7, 7), IsWhite = true };
            board[1, 7] = new Knight() { Position = new Point(1, 7), IsWhite = true };
            board[6, 7] = new Knight() { Position = new Point(6, 7), IsWhite = true };
            board[2, 7] = new Bishop() { Position = new Point(2, 7), IsWhite = true };
            board[5, 7] = new Bishop() { Position = new Point(5, 7), IsWhite = true };
            board[3, 7] = new Queen() { Position = new Point(3, 7), IsWhite = true };
            board[4, 7] = new King() { Position = new Point(4, 7), IsWhite = true };

            for (int i = 0; i < 8; i++)
            {
                board[i, 1] = new Pawn() { Position = new Point(i, 1), IsWhite = false };
            }
            board[0, 0] = new Tower() { Position = new Point(0, 0), IsWhite = false };
            board[7, 0] = new Tower() { Position = new Point(7, 0), IsWhite = false };
            board[1, 0] = new Knight() { Position = new Point(1, 0), IsWhite = false };
            board[6, 0] = new Knight() { Position = new Point(6, 0), IsWhite = false };
            board[2, 0] = new Bishop() { Position = new Point(2, 0), IsWhite = false };
            board[5, 0] = new Bishop() { Position = new Point(5, 0), IsWhite = false };
            board[3, 0] = new Queen() { Position = new Point(3, 0), IsWhite = false };
            board[4, 0] = new King() { Position = new Point(4, 0), IsWhite = false };

            foreach (var item in board)
            {
                if(item == null) continue;
                if (item.IsWhite)
                    WhiteFigures.Add(item);
                else BlackFigures.Add(item);
            }

            if(GameStarting != null)
                GameStarting(null, null);
        }

        public static void Move(Point start, Point end)
        {
            if (board[start.X, start.Y] == null)
                throw new NullReferenceException();
            ToweringPos = null;
            if (CanMove(start, end))
            {
                if (board[start.X, start.Y] is Pawn)
                    (board[start.X, start.Y] as Pawn).FirstMove = false;
                else if (board[start.X, start.Y] is Tower)
                    (board[start.X, start.Y] as Tower).FirstMove = false;
                else if (board[start.X, start.Y] is King && (board[start.X, start.Y] as King).FirstMove)
                {
                    (board[start.X, start.Y] as King).FirstMove = false;
                    if (end.X == 1)
                    {
                        board[2, start.Y] = board[0, start.Y];
                        board[0, start.Y] = null;
                        board[2, start.Y].Position = new Point(2, start.Y);
                        ToweringPos = new Point(2, start.Y);
                    }
                    else if (end.X == 6)
                    {
                        board[5, start.Y] = board[7, start.Y];
                        board[7, start.Y] = null;
                        board[5, start.Y].Position = new Point(5, start.Y);
                        ToweringPos = new Point(5, start.Y);
                    }
                }

                if (board[end.X, end.Y] != null)
                {
                    if (board[end.X, end.Y].IsWhite)
                    {
                        WhiteFigures.Remove(board[end.X, end.Y]);
                        killedWhite.Add(board[end.X, end.Y]);
                        if (WhiteKilled != null)
                            WhiteKilled.Invoke(board[end.X, end.Y], null);
                    }
                    else
                    {
                        BlackFigures.Remove(board[end.X, end.Y]);
                        killedBlack.Add(board[end.X, end.Y]);
                        if (BlackKilled != null)
                            BlackKilled.Invoke(board[end.X, end.Y], null);
                    }
                }

                board[end.X, end.Y] = board[start.X, start.Y];
                board[start.X, start.Y] = null;
                board[end.X, end.Y].Position = new Point(end.X, end.Y);

                OnFigureMove(board[end.X, end.Y]);
                
                WhiteTurn = !WhiteTurn;

                GameModel gameModel = new GameModel()
                {
                    WhiteTime = WhiteTime.ToString(),
                    BlackTime = BlackTime.ToString(),
                    Figures = WhiteFigures.Select(f => FigureDataModel.ToFigureDataModel(f))
                    .Concat(BlackFigures.Select(f => FigureDataModel.ToFigureDataModel(f))).ToList(),

                    BlackKilled = killedBlack.Select(f => FigureDataModel.ToFigureDataModel(f)).ToList(),
                    WhiteKilled = killedWhite.Select(f => FigureDataModel.ToFigureDataModel(f)).ToList(),
                    LogsImg = Logs.Select(x => x.Item1).ToList(),
                    LogsPos = Logs.Select(x => x.Item2).ToList(),
                    WhiteTurn = WhiteTurn
                };
                XmlDataBase.Write(currentGamePath, gameModel);

                if (ChessGame.IsDraw(end))
                {
                    EndGame(GameState.Draw);
                }
                if (ChessGame.IsCheck(out Point position) && ChessGame.IsCheckMate(position))
                {
                    EndGame((WhiteTurn ? GameState.Black : GameState.White));
                }
            }
        }

        public static bool IsCheck(out Point position)
        {
            position = new Point(-1, -1);
            foreach(var figure in WhiteFigures)
            {
                if (GetAttacks(figure.Position).Contains(BlackFigures.Find(f => f is King).Position))
                {
                    position = BlackFigures.Find(f => f is King).Position;
                    return true;
                }
            }

            foreach (var figure in BlackFigures)
            {
                if (GetAttacks(figure.Position).Contains(WhiteFigures.Find(f => f is King).Position))
                {
                    position = WhiteFigures.Find(f => f is King).Position;
                    return true;
                }
            }

            return false;
        }

        private static bool IsCheck(bool isWhite)
        {
            if (!isWhite)
            {
                foreach (var figure in WhiteFigures)
                {
                    if (GetAttacks(figure.Position, true).Contains(BlackFigures.Find(f => f is King).Position))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                foreach (var figure in BlackFigures)
                {
                    if (GetAttacks(figure.Position, true).Contains(WhiteFigures.Find(f => f is King).Position))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private static bool CanMoveIfCheck(Point position, Point point)
        {
            var figure1 = board[position.X, position.Y];
            var figure2 = board[point.X, point.Y];
            board[position.X, position.Y] = null;
            board[point.X, point.Y] = figure1;
            figure1.Position = point;
            bool canGo = !IsCheck(figure1.IsWhite);

            board[position.X, position.Y] = figure1;
            figure1.Position = position;
            board[point.X, point.Y] = figure2;
            return canGo;
        }

        public static bool IsCheckMate(Point position)
        {
            if (IsCheck(board[position.X, position.Y].IsWhite))
            {
                if(board[position.X, position.Y].IsWhite)
                {
                    foreach(var figure in WhiteFigures)
                    {
                        if (GetMoves(figure.Position).Count > 0 || GetAttacks(figure.Position).Count > 0)
                            return false;
                    }
                    return true;
                }
                else
                {
                    foreach (var figure in BlackFigures)
                    {
                        if (GetMoves(figure.Position).Count > 0 || GetAttacks(figure.Position).Count > 0)
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public static List<Point> GetMoves(Point position)
        {
            List<List<Point>> list = null;
            try
            {
                list = board[position.X, position.Y].GetMoves();
            }
            catch(NullReferenceException e)
            {
                throw;
            }
            List<Point> result = new List<Point>();    
            foreach(var move in list)
                foreach(var point in move)
                {
                    if (point.X < 0 || point.Y < 0 || point.X > 7 || point.Y > 7) break;
                    if (board[point.X, point.Y] != null)
                        break;

                    bool canGo = CanMoveIfCheck(position, point);

                    if(canGo)
                        result.Add(point);
                }

            if(board[position.X, position.Y] is King)
            {
                if((board[position.X, position.Y] as King).FirstMove)
                {
                    if (board[0, position.Y] != null && board[0, position.Y] is Tower && (board[0, position.Y] as Tower).FirstMove
                        && (board[0, position.Y] as Tower).IsWhite == (board[position.X, position.Y] as King).IsWhite)
                    {
                        if (board[1, position.Y] == null && board[2, position.Y] == null && board[3, position.Y] == null)
                        {
                            bool canGo = CanMoveIfCheck(position, new Point(1, position.Y));

                            if (canGo)
                                result.Add(new Point(1, position.Y));
                        }
                            
                    }
                    if(board[7, position.Y] != null && board[7, position.Y] is Tower && (board[7, position.Y] as Tower).FirstMove
                        && (board[7, position.Y] as Tower).IsWhite == (board[position.X, position.Y] as King).IsWhite)
                    {
                        if (board[6, position.Y] == null && board[5, position.Y] == null)
                        {
                            bool canGo = CanMoveIfCheck(position, new Point(6, position.Y));

                            if (canGo)
                                result.Add(new Point(6, position.Y));
                        }
                    }
                }
            }

            return result;
        }

        // доробити щоб не можнв було бити під час шаху
        public static List<Point> GetAttacks(Point position, bool check = false)
        {
            if (board[position.X, position.Y] is Pawn)
            {
                List<Point> _result = new List<Point>();
                if (board[position.X, position.Y].IsWhite)
                {
                    if (position.X > 0 && position.Y > 0 && board[position.X - 1, position.Y - 1] != null &&
                        !board[position.X - 1, position.Y - 1].IsWhite)
                        _result.Add(new Point(position.X - 1, position.Y - 1));
                    if (position.X < 7 && position.Y > 0 && board[position.X + 1, position.Y - 1] != null &&
                        !board[position.X + 1, position.Y - 1].IsWhite)
                        _result.Add(new Point(position.X + 1, position.Y - 1));
                }
                else
                {
                    if (position.X > 0 && position.Y < 7 && board[position.X - 1, position.Y + 1] != null &&
                        board[position.X - 1, position.Y + 1].IsWhite)
                        _result.Add(new Point(position.X - 1, position.Y + 1));
                    if (position.X < 7 && position.Y < 7 && board[position.X + 1, position.Y + 1] != null &&
                        board[position.X + 1, position.Y + 1].IsWhite)
                        _result.Add(new Point(position.X + 1, position.Y + 1));
                }

                if (!check)
                {
                    var _tempResult = new List<Point>(_result);
                    foreach(var move in _tempResult)
                    {
                        if(!CanMoveIfCheck(position, move))
                            _result.Remove(move);
                    }
                }

                return _result;
            }
            List<List<Point>> list = null;
            List<Point> result = new List<Point>();

            if (board[position.X, position.Y] == null) return result;
            
            list = board[position.X, position.Y].GetMoves();
            
            foreach (var move in list)
                foreach (var point in move)
                {
                    if (point.X < 0 || point.Y < 0 || point.X > 7 || point.Y > 7) break;
                    if (board[point.X, point.Y] != null)
                    {
                        if (board[point.X, point.Y].IsWhite != board[position.X, position.Y].IsWhite)
                        {
                            result.Add(point);
                            break;
                        }
                        else break;
                    }
                }

            if (!check)
            {
                var tempResult = new List<Point>(result);
                foreach (var move in tempResult)
                {
                    if (!CanMoveIfCheck(position, move))
                        result.Remove(move);
                }
            }

            return result;
        }
    }
}
