namespace Game
{
    public class CellModel
    {
        private int x;
        private int y;
        public CellState CellState { get; set; }

        public CellModel( int x, int y,CellState cellState)
        {
            this.CellState = cellState;
            this.x = x;
            this.y = y;
        }

        public int X => x;

        public int Y => y;
    }
}