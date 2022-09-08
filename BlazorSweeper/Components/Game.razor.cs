using BerldSweeper;
using BerldSweeperEngine;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorSweeper.Components
{
    public partial class Game
    {
        private readonly int _width = 30;
        private readonly int _height = 16;
        private readonly int _bombAmount = 99;

        private MineSweeper sweeper;

        private Engine engine;


        public Game()
        {
            engine = new Engine();
            sweeper = CreateNewGame();
        }

        private int GetRemainingFlags() => sweeper.BombAmount - sweeper.FlagAmount;

        private void ResetGame() => sweeper = CreateNewGame();

        private MineSweeper CreateNewGame() => new(_width, _height, _bombAmount);

        private void ChildSquareClicked((MouseEventArgs e, Square square) args)
        {
            if (args.e.Button == 0)
            {
                sweeper.Reveal(args.square);
            }
            else
            {
                sweeper.ToggleFlag(args.square);
            }
        }

        private void ExecuteSweepActions(List<SweepAction> actions)
        {
            foreach (SweepAction action in actions)
            {
                if (action.ActionType == SweepActionType.Reveal)
                {
                    sweeper.SetFlagged(action.Square, false);
                    sweeper.Reveal(action.Square);
                }
                else if (action.ActionType == SweepActionType.Flag)
                {
                    sweeper.SetFlagged(action.Square, true);
                }
            }
        }

        private void SweepSuffocationReveals()
        {
            List<SweepAction> actions = engine.SolveSuffocationReveals(sweeper);
            ExecuteSweepActions(actions);
        }

        private void SweepTrivialFlags()
        {
            List<SweepAction> actions = engine.SolveTrivialFlags(sweeper);
            ExecuteSweepActions(actions);
        }

        private void SweepTrivialReveals()
        {
            List<SweepAction> actions = engine.SolveTrivialReveals(sweeper);
            ExecuteSweepActions(actions);
        }
    }
}
