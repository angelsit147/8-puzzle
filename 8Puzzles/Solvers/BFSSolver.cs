using System.Collections.Generic;
using System.Linq;

namespace _8Puzzles.Solvers
{
    public class BFSSolver : ISolver
    {
        private const int N = 3;

        private static readonly int[] row = { 0, 0, -1, 1 };
        private static readonly int[] col = { -1, 1, 0, 0 };

        public int RenderDelay { get; }

        // Конструктор: Ініціалізує солвер із заданою затримкою рендерингу
        public BFSSolver(int renderDelay)
        {
            RenderDelay = renderDelay;
        }

        // Перевіряє, чи є поточний стан дошки цільовим (розв’язаним)
        private static bool IsGoalState(int[,] board)
        {
            int[,] goal = new int[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (board[i, j] != goal[i, j])
                        return false;
                }
            }
            return true;
        }

        // Перевіряє, чи є координати дійсними в межах дошки
        private static bool IsValid(int x, int y)
        {
            return x >= 0 && x < N && y >= 0 && y < N;
        }

        // Перетворює дошку в рядкове представлення для порівняння станів
        private static string BoardToString(int[,] board)
        {
            return string.Join("|", Enumerable.Range(0, N).Select(i =>
                string.Join(",", Enumerable.Range(0, N).Select(j => board[i, j]))));
        }

        // Формує список станів дошки, що ведуть до розв’язку
        private static List<int[,]> GetSolutionPath(PuzzleState goalState)
        {
            List<int[,]> path = new List<int[,]>();
            PuzzleState current = goalState;

            while (current != null)
            {
                path.Add(current.Board);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }

        // Розв’язує головоломку за допомогою алгоритму BFS
        public (int, List<int[,]>) SolvePuzzle(int[,] start, int x, int y)
        {
            Queue<PuzzleState> queue = new Queue<PuzzleState>();
            HashSet<string> visited = new HashSet<string>();

            PuzzleState initialState = new PuzzleState(start, x, y, 0);
            queue.Enqueue(initialState);
            int totalIterations = 1;
            visited.Add(BoardToString(start));

            while (queue.Count > 0)
            {
                PuzzleState current = queue.Dequeue();

                if (IsGoalState(current.Board))
                {
                    return (totalIterations, GetSolutionPath(current));
                }

                for (int i = 0; i < 4; i++)
                {
                    int newX = current.X + row[i];
                    int newY = current.Y + col[i];

                    if (IsValid(newX, newY))
                    {
                        int[,] newBoard = (int[,])current.Board.Clone();
                        newBoard[current.X, current.Y] = newBoard[newX, newY];
                        newBoard[newX, newY] = 0;

                        string newBoardKey = BoardToString(newBoard);
                        if (!visited.Contains(newBoardKey))
                        {
                            visited.Add(newBoardKey);
                            queue.Enqueue(new PuzzleState(newBoard, newX, newY, current.Depth + 1, current));
                            totalIterations++;
                        }
                    }
                }
            }

            return (totalIterations, new List<int[,]>());
        }
    }
}