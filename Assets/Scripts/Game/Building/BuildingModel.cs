using System;
using Newtonsoft.Json;

namespace Game
{
    [Serializable]
    public class BuildingModel
    {
        [JsonProperty]
        private int size;
        [JsonProperty]
        private float x;
        [JsonProperty]
        private float y;
        [JsonIgnore]
        public float X => x;
        [JsonIgnore]
        public float Y => y;
        

        [JsonIgnore]
        private BuildingView buildingView;
        
        [JsonIgnore]
        public BuildingView BuildingView
        {
            get => buildingView;
            set => buildingView = value;
        }

        public BuildingModel(int size)
        {
            this.size = size;
        }

        public BuildingModel(int size, float x, float y)
        {
            this.size = size;
            this.x = x;
            this.y = y;
        }

        public BuildingModel()
        {
           
        }

        [JsonIgnore]
        public int Size
        {
            get => size;
            set => size = value;
        }

        public void SaveVisualPosition()
        {
            x = BuildingView.transform.position.x;
            y = BuildingView.transform.position.z;
        }

       
    }
}