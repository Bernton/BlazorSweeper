using BerldSweeper;

namespace BerldSweeperEngine
{
    public class Engine
    {
        public List<SweepAction> SolveTrivialFlags(MineSweeper sourceGame)
        {
            EngineMineSweeper game = new(sourceGame);
            SetTrivialFlags(game);
            var flaggedSquares = game.Squares.Where(c => c.IsFlag);
            return flaggedSquares.Select(c => new SweepAction(c.Source, SweepActionType.Flag)).ToList();
        }

        public List<SweepAction> SolveTrivialReveals(MineSweeper sourceGame)
        {
            List<SweepAction> actions = new();

            EngineMineSweeper game = new(sourceGame);
            SetTrivialFlags(game);

            if (game.Source.State != SweepState.Sweeping)
            {
                return actions;
            }

            foreach (EngineSquare square in game.Squares)
            {
                if (square.Source.Value is NumberSquare numberSquare)
                {
                    int flaggedNeighbors = square.Neighbors.Count(c => c.IsFlag);

                    if (numberSquare.Number == flaggedNeighbors)
                    {
                        foreach (EngineSquare neighbor in square.Neighbors)
                        {
                            if (neighbor.IsUnknown)
                            {
                                actions.Add(new SweepAction(neighbor.Source, SweepActionType.Reveal));
                            }
                        }
                    }
                }
            }

            return actions;
        }

        private static void SetTrivialFlags(EngineMineSweeper game)
        {
            foreach (EngineSquare square in game.Squares)
            {
                if (square.Source.Value is NumberSquare numberSquare)
                {
                    int hiddenNeighbors = square.Neighbors.Count(c => c.IsHidden);

                    if (numberSquare.Number == hiddenNeighbors)
                    {
                        foreach (EngineSquare neighbor in square.Neighbors)
                        {
                            if (neighbor.IsHidden)
                            {
                                neighbor.IsFlag = true;
                            }
                        }
                    }
                }
            }
        }
    }
}