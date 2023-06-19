using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models
{
    public class Knight : IFigure
    {
        public Point Position { get; set; }
        public bool IsWhite { get; set; }

        public List<List<Point>> GetMoves()
        {
            return new List<List<Point>>()
            {
                new List<Point> { new Point(Position.X+1, Position.Y-2) },
                new List<Point> { new Point(Position.X+2, Position.Y-1) },
                new List<Point> { new Point(Position.X+2, Position.Y+1) },
                new List<Point> { new Point(Position.X+1, Position.Y+2) },
                new List<Point> { new Point(Position.X-1, Position.Y+2) },
                new List<Point> { new Point(Position.X-2, Position.Y+1) },
                new List<Point> { new Point(Position.X-2, Position.Y-1) },
                new List<Point> { new Point(Position.X-1, Position.Y-2) },
            };
        }
    }
}
