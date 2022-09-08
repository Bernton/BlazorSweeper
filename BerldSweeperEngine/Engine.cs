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
            var flagActions = flaggedSquares.Select(c => new SweepAction(c.Source, SweepActionType.Flag));
            return ValidateRemoveDuplicates(flagActions.ToList());
        }

        public List<SweepAction> SolveTrivialReveals(MineSweeper sourceGame)
        {
            List<SweepAction> actions = new();
            EngineMineSweeper game = new(sourceGame);

            if (game.Source.State != SweepState.Sweeping)
            {
                return actions;
            }

            SetTrivialFlags(game);

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

            return ValidateRemoveDuplicates(actions);
        }

        public List<SweepAction> SolveSuffocationReveals(MineSweeper sourceGame)
        {
            List<SweepAction> actions = new();
            EngineMineSweeper game = new(sourceGame);

            if (game.Source.State != SweepState.Sweeping)
            {
                return actions;
            }

            SetTrivialFlags(game);

            foreach (EngineSquare square in game.Squares)
            {
                if (square.IsUnknown)
                {
                    square.IsFlag = true;

                    List<EngineSquare> numberNeighbors = square.Neighbors.Where(c => c.Source.Value is NumberSquare).ToList();

                    if (numberNeighbors.Count > 0)
                    {
                        foreach (EngineSquare numberNeighbor in numberNeighbors)
                        {
                            if (CheckFilledNumberSquares(numberNeighbor))
                            {
                                actions.Add(new SweepAction(square.Source, SweepActionType.Reveal));
                            }
                        }
                    }

                    square.IsFlag = false;
                }
            }

            return ValidateRemoveDuplicates(actions);
        }

        private bool CheckFilledNumberSquares(EngineSquare numberSquare)
        {
            bool suffocationFound = false;

            if (numberSquare.Source.Value is not NumberSquare value)
            {
                throw new ArgumentException("Must be NumberSquare.");
            }

            int flaggedNeighbors = numberSquare.Neighbors.Count(c => c.IsFlag);

            if (value.Number - flaggedNeighbors == 0)
            {
                List<EngineSquare> potentialSuffocationSquares = new();
                List<EngineSquare> saveSquares = numberSquare.Neighbors.Where(c => c.IsUnknown).ToList();

                foreach (EngineSquare saveSquare in saveSquares)
                {
                    var potentialSquares = saveSquare.Neighbors.Where(c => c.Source.Value is NumberSquare);
                    potentialSuffocationSquares.AddRange(potentialSquares);
                }

                if (AnyNumberSquareSuffocated(potentialSuffocationSquares, saveSquares)) {
                    suffocationFound = true;
                }
            }

            return suffocationFound;
        }

        private static bool AnyNumberSquareSuffocated(List<EngineSquare> numberSquares, List<EngineSquare> saveSquares)
        {
            foreach (EngineSquare numberSquare in numberSquares)
            {
                if (numberSquare.Source.Value is not NumberSquare value)
                {
                    throw new ArgumentException("Must be NumberSquare.");
                }

                int maxBombSquareCount = numberSquare.Neighbors.Count(c => c.IsFlag || (c.IsUnknown && !saveSquares.Contains(c)));

                if (value.Number - maxBombSquareCount > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static List<SweepAction> ValidateRemoveDuplicates(List<SweepAction> actions)
        {
            List<SweepAction> validatedActions = new();

            foreach (SweepAction action in actions)
            {
                bool foundDuplicate = false;

                foreach (SweepAction validatedAction in validatedActions)
                {
                    if (action.Square == validatedAction.Square)
                    {
                        if (action.ActionType != validatedAction.ActionType)
                        {
                            throw new InvalidOperationException("Multiple actions with same square but other action type.");
                        }

                        foundDuplicate = true;
                        break;
                    }
                }

                if (!foundDuplicate)
                {
                    validatedActions.Add(action);
                }
            }

            return validatedActions;
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