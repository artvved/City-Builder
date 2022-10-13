using System;
using Newtonsoft.Json;

namespace Game
{
    [Serializable]
    public class CellModel
    {
        [JsonIgnore] private CellView cellView;

        [JsonProperty]
        private int x;
        [JsonProperty]
        private int y;

        [JsonIgnore]
        public int X => x;
        [JsonIgnore]
        public int Y => y;
        
        [JsonProperty]
        public CellState CellState { get; set; }

        
        public BuildingModel BuildingModel { get; set; }

        [JsonIgnore]
        public CellView CellView
        {
            get => cellView;
            set => cellView = value;
        }


        public CellModel(int x, int y, CellState cellState)
        {
            this.CellState = cellState;
            this.x = x;
            this.y = y;
        }
    }
}