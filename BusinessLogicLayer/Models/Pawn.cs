using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models
{
    public class Pawn : IFigure
    {
        public bool FirstMove = true;

        public Point Position { get; set; }
        public bool IsWhite { get; set; }

        public List<List<Point>> GetMoves()
        {
            int direction = IsWhite ? 1 : -1;
            if (FirstMove)
                return new List<List<Point>>() { new List<Point>(){ new Point(Position.X, Position.Y-(direction)), 
                    new Point(Position.X, Position.Y - (direction * 2)), } };
            else return new List<List<Point>>() { new List<Point>(){ new Point(Position.X, Position.Y-(direction)) } };
        }
    }
}
