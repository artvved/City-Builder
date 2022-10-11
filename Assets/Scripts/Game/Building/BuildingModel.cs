namespace Game
{
    public class BuildingModel
    {
        private int size;

        public BuildingModel(int size)
        {
            this.size = size;
        }

        public int Size
        {
            get => size;
            set => size = value;
        }
    }
}