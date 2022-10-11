namespace Game
{
    public class CellModel
    {
        private int x;
        public int X => x;
        
        private int y;
        public int Y => y;
        
        public CellState CellState { get; set; }
        public BuildingModel BuildingModel { get; set; }

        public CellModel( int x, int y,CellState cellState)
        {
            this.CellState = cellState;
            this.x = x;
            this.y = y;
        }

       
    }
}