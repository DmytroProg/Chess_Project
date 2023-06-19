using BusinessLogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public static class ChessConverter
    {
        public static string ConvertFigureToSource(IFigure item)
        {
            string path = "";
            if (item is Pawn) path = "pawn";
            else if (item is Tower) path = "tower";
            else if (item is Bishop) path = "bishop";
            else if (item is Knight) path = "knight";
            else if (item is Queen) path = "queen";
            else if (item is King) path = "king";

            if (!item.IsWhite)
            {
                var builder = new StringBuilder(path);
                builder.Insert(0, "b_");
                path = builder.ToString();
            }
            return path;
        }
    }
}
