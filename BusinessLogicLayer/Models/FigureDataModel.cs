using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models
{
    [Serializable]
    public class FigureDataModel
    {
        public Point Position { get; set; }
        public string FigureType { get; set; }
        public bool IsWhite { get; set; }
        public bool FirstMove { get; set; }

        public static FigureDataModel ToFigureDataModel(IFigure figure)
        {
            FigureDataModel result = new FigureDataModel();
            result.Position = figure.Position;
            result.IsWhite = figure.IsWhite;
            result.FigureType = figure.GetType().ToString();
            if (figure is Pawn) result.FirstMove = (figure as Pawn).FirstMove;
            else if (figure is Tower) result.FirstMove = (figure as Tower).FirstMove;
            else if (figure is King) result.FirstMove = (figure as King).FirstMove;

            return result;
        }

        public static IFigure ToFigure(FigureDataModel model)
        {
            if(model.FigureType == typeof(Pawn).ToString())
                return new Pawn() { Position = model.Position, IsWhite = model.IsWhite, FirstMove = model.FirstMove };
            if (model.FigureType == typeof(Tower).ToString())
                return new Tower() { Position = model.Position, IsWhite = model.IsWhite, FirstMove = model.FirstMove };
            if (model.FigureType == typeof(King).ToString())
                return new King() { Position = model.Position, IsWhite = model.IsWhite, FirstMove = model.FirstMove };
            if (model.FigureType == typeof(Bishop).ToString())
                return new Bishop() { Position = model.Position, IsWhite = model.IsWhite };
            if (model.FigureType == typeof(Knight).ToString())
                return new Knight() { Position = model.Position, IsWhite = model.IsWhite };
            if (model.FigureType == typeof(Queen).ToString())
                return new Queen() { Position = model.Position, IsWhite = model.IsWhite };
            return null;
        }
    }
}
