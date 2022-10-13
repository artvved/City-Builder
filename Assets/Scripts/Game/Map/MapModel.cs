using System;
using Newtonsoft.Json;

namespace Game
{
    [Serializable]
    public class MapModel
    {
        private int width;
        private int height;
        private CellModel[,] map;

        
        public int Width => width;
        public int Height => height;

        public CellModel[,] Map
        {
            get => map;
            set => map = value;
        }

        public MapModel(int width, int height)
        {
            this.width = width;
            this.height = height;
            map = new CellModel[this.width, this.height];
        }

       
    }
}