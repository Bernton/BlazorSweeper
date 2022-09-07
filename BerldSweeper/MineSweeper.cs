namespace BerldSweeper
{
    public class MineSweeper
    {
        public Square[,] Field { get; }
        public List<Square> Squares { get; }

        public SweepState State { get; private set; }
        public int Width { get; }
        public int Height { get; }
        public int BombAmount { get; }
        public int FlagAmount => Squares.Count(c => c.State == SquareState.Flagged);

        private bool GameFinished => State == SweepState.Solved || State == SweepState.Dead;

        public MineSweeper(int width, int height, int bombAmount)
        {
            if (bombAmount >= width * height)
            {
                throw new ArgumentException($"{nameof(bombAmount)} must be smaller than the number of squares.");
            }

            State = SweepState.Initial;
            Width = width;
            Height = height;
            BombAmount = bombAmount;
            Field = new Square[Height, Width];
            InitializeField();
            Squares = Field.Cast<Square>().ToList();
        }

        public void ResetFlags()
        {
            foreach (Square square in Squares)
            {
                SetFlagged(square, false);
            }
        }

        public void SetFlagged(Square square, bool isFlagged)
        {
            if (GameFinished)
            {
                return;
            }

            if (square.State == SquareState.Hidden && isFlagged)
            {
                square.State = SquareState.Flagged;
            }
            else if (square.State == SquareState.Flagged && !isFlagged)
            {
                square.State = SquareState.Hidden;
            }
        }

        public void ToggleFlag(Square square)
        {
            if (GameFinished)
            {
                return;
            }

            SetFlagged(square, square.State != SquareState.Flagged);
        }

        public void Reveal(Square square)
        {
            if (GameFinished)
            {
                return;
            }

            if (State == SweepState.Sweeping)
            {
                RevealSquareRecursively(square);
            }
            else if (State == SweepState.Initial)
            {
                RevealInitialCell(square);
            }

            if (Squares.TrueForAll(c => c.RealValue is BombSquare || c.State == SquareState.Revealed))
            {
                State = SweepState.Solved;
            }
        }

        private void RevealSquareRecursively(Square square)
        {
            if (square.State != SquareState.Hidden)
            {
                return;
            }

            square.State = SquareState.Revealed;

            if (square.RealValue is BombSquare)
            {
                State = SweepState.Dead;
            }
            else if (square.RealValue is EmptySquare)
            {
                var neighbors = GetNeighbors(square);

                foreach (var neighbor in neighbors)
                {
                    RevealSquareRecursively(neighbor);
                }
            }
        }

        private void RevealInitialCell(Square square)
        {
            State = SweepState.Sweeping;

            List<Square> exludedSquares = GetNeighbors(square);
            exludedSquares.Add(square);

            SetBombs(Squares.Except(exludedSquares).ToList());
            SetNumbers();
            RevealSquareRecursively(square);
        }

        private void InitializeField()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Field[y, x] = new Square(x, y, new EmptySquare());
                }
            }
        }

        private void SetBombs(List<Square> squares)
        {
            Random random = new();

            for (int i = 0; i < BombAmount; i++)
            {
                SetBomb(random, squares);
            }
        }

        private static void SetBomb(Random random, List<Square> squares)
        {
            int chosenIndex = random.Next(squares.Count);
            squares[chosenIndex].RealValue = new BombSquare();
            squares.RemoveAt(chosenIndex);
        }

        private void SetNumbers()
        {
            foreach (Square square in Squares)
            {
                if (square.RealValue is not EmptySquare)
                {
                    continue;
                }

                List<Square> neighbors = GetNeighbors(square);
                int bombSqaureNeighborAmount = neighbors.Count(c => c.RealValue is BombSquare);

                if (bombSqaureNeighborAmount > 0)
                {
                    square.RealValue = new NumberSquare(bombSqaureNeighborAmount);
                }
            }
        }

        public List<Square> GetNeighbors(Square square)
        {
            List<Square> neighbors = new();

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    int neighborX = square.X + x;
                    int neighborY = square.Y + y;

                    if (neighborX == square.X && neighborY == square.Y)
                    {
                        continue;
                    }

                    if (neighborX < 0 || neighborX >= Width || neighborY < 0 || neighborY >= Height)
                    {
                        continue;
                    }

                    neighbors.Add(Field[neighborY, neighborX]);
                }
            }

            return neighbors;
        }
    }
}
