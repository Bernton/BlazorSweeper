using BerldSweeper;

namespace BerldSweeperEngine
{
    internal class EngineSquare
    {
        private bool _isFlag;

        public List<EngineSquare> Neighbors { get; set; }
        public Square Source { get; }

        public bool IsFlag
        {
            get => _isFlag;
            set
            {
                if (IsRevealed && value)
                {
                    throw new InvalidOperationException("Can not set flagged on revealed square.");
                }

                _isFlag = value;
            }
        }

        public bool IsUnknown => !IsRevealed && !IsFlag;
        public bool IsHidden => !IsRevealed;

        public bool IsRevealed => Source.State == SquareState.Revealed;

        public EngineSquare(Square source)
        {
            Source = source;
            _isFlag = false;
            Neighbors = new List<EngineSquare>();
        }
    }
}
