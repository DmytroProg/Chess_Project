using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Models
{
    [Serializable]
    public class GameModel
    {
        public string WhiteTime { get; set; }
        public string BlackTime { get; set; }

        public List<FigureDataModel> Figures { get; set; }
        public List<FigureDataModel> WhiteKilled { get; set; }
        public List<FigureDataModel> BlackKilled { get; set; }

        public List<string> LogsImg { get; set; }
        public List<string> LogsPos { get; set; }

        public bool WhiteTurn { get; set; }

        public GameModel()
        {
            Figures = new List<FigureDataModel>();
            WhiteKilled = new List<FigureDataModel>();
            BlackKilled = new List<FigureDataModel>();
            LogsImg = new List<string>();
            LogsPos = new List<string>();
        }
    }
}
