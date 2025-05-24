using System.Collections.Generic;

namespace _8Puzzles.Solvers
{
    public class PuzzleState
    {
        public int[,] Board { get; }
        public int X { get; }
        public int Y { get; }
        public int Depth { get; }
        public PuzzleState Parent { get; }

        public HashSet<string> Visited { get; }

        // Конструктор: Ініціалізує стан головоломки з дошки, координатами, глибиною та батьківським станом.
        public PuzzleState(int[,] board, int x, int y, int depth, PuzzleState parent = null)
        {
            Board = (int[,])board.Clone();
            X = x;
            Y = y;
            Depth = depth;
            Parent = parent;
        }
        // Конструктор: Ініціалізує стан головоломки з дошки, координатами, глибиною, множиною відвідуваних станів та батьківським станом.
        public PuzzleState(int[,] board, int x, int y, int depth, HashSet<string> visited, PuzzleState parent = null)
        {
            Board = (int[,])board.Clone();
            X = x;
            Y = y;
            Depth = depth;
            Parent = parent;
            Visited = visited;
        }
    }
}
