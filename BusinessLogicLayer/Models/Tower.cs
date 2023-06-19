using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models
{
    public class Tower : IFigure
    {
        public Point Position { get; set; }
        public bool IsWhite { get; set; }
        public bool FirstMove = true;

        public List<List<Point>> GetMoves()
        {
            var list = new List<List<Point>>();
            for (int i = 0; i < 4; i++)
                list.Add(new List<Point>());
            for (int i = 1; i < 8; i++)
            {
                list[0].Add(new Point(Position.X, Position.Y + i));
                list[1].Add(new Point(Position.X, Position.Y - i));
                list[2].Add(new Point(Position.X + i, Position.Y));
                list[3].Add(new Point(Position.X - i, Position.Y));
            }
            return list;
        }
    }
}
