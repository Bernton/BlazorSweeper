using BerldSweeper;

namespace BerldSweeperEngine
{
    internal class EngineMineSweeper
    {
        internal EngineSquare[,] Field { get; }
        internal List<EngineSquare> Squares { get; }
        public MineSweeper Source { get; }

        internal EngineMineSweeper(MineSweeper source)
        {
            Source = source;
            Field = new EngineSquare[source.Height, source.Width];

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Field[y, x] = new EngineSquare(source.Field[y, x]);
                }
            }

            Squares = Field.Cast<EngineSquare>().ToList();

            foreach (EngineSquare square in Squares)
            {
                square.Neighbors = GetNeighbors(square);
            }
        }

        private List<EngineSquare> GetNeighbors(EngineSquare square)
        {
            List<EngineSquare> neighbors = new();

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    int neighborX = square.Source.X + x;
                    int neighborY = square.Source.Y + y;

                    if (neighborX == square.Source.X && neighborY == square.Source.Y)
                    {
                        continue;
                    }

                    if (neighborX < 0 || neighborX >= Source.Width || neighborY < 0 || neighborY >= Source.Height)
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
