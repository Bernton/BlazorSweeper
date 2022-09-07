namespace BerldSweeper
{
    public class Square
    {
        public SquareValue? Value => State == SquareState.Revealed ? RealValue : null;

        public SquareState State { get; internal set; }
        public int X { get; }
        public int Y { get; }

        internal SquareValue RealValue { get; set; }

        internal Square(int x, int y, SquareValue value)
        {
            State = SquareState.Hidden;
            X = x;
            Y = y;
            RealValue = value;
        }
    }
}
