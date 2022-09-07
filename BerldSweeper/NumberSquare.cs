namespace BerldSweeper
{
    public class NumberSquare : SquareValue
    {
        private int _number;

        internal NumberSquare(int number)
        {
            Number = number;
        }

        public int Number
        {
            get
            {
                return _number;
            }

            internal set
            {
                if (value < 1 || value > 8)
                {
                    throw new ArgumentOutOfRangeException(nameof(Number), $"{nameof(Number)} must be higher than 0 and lower than 9.");
                }

                _number = value;
            }
        }
    }
}
