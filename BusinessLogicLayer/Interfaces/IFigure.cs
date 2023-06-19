using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BusinessLogicLayer.Models
{
    public interface IFigure
    {
        Point Position { get; set; }
        bool IsWhite { get; set; }

        List<List<Point>> GetMoves();
    }
}
