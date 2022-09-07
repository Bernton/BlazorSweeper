using BerldSweeper;

namespace BerldSweeperEngine
{
    public class SweepAction
    {
        public Square Square { get; }
        public SweepActionType ActionType { get; }

        internal SweepAction(Square square, SweepActionType actionType)
        {
            Square = square;
            ActionType = actionType;
        }
    }
}
