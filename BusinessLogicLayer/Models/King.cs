using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models
{
    public class King : IFigure
    {
        public Point Position { get; set; }
        public bool IsWhite { get; set; }
        public bool FirstMove = true;

        public List<List<Point>> GetMoves()
        {
            var list = new List<List<Point>>();
            for (int i = 0; i < 8; i++)
                list.Add(new List<Point>());

            list[0].Add(new Point(Position.X + 1, Position.Y + 1));
            list[1].Add(new Point(Position.X - 1, Position.Y - 1));
            list[2].Add(new Point(Position.X + 1, Position.Y - 1));
            list[3].Add(new Point(Position.X - 1, Position.Y + 1));
            list[4].Add(new Point(Position.X, Position.Y + 1));
            list[5].Add(new Point(Position.X, Position.Y - 1));
            list[6].Add(new Point(Position.X + 1, Position.Y));
            list[7].Add(new Point(Position.X - 1, Position.Y));

            return list;
        }
    }
}
