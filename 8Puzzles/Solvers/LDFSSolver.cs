using System.Collections.Generic;
using System.Linq;

namespace _8Puzzles.Solvers
{
    public class LDFSSolver : ISolver
    {
        private const int N = 3;

        private static readonly int[] row = { 0, 0, -1, 1 };
        private static readonly int[] col = { -1, 1, 0, 0 };

        private int depthLimit;

        public int RenderDelay { get; }

        // Конструктор: Ініціалізує солвер із заданою межею глибини та затримкою рендерингу
        public LDFSSolver(int depthLimit, int renderDelay)
        {
            this.depthLimit = depthLimit;
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

        // Розв’язує головоломку за допомогою алгоритму LDFS
        public (int, List<int[,]>) SolvePuzzle(int[,] start, int x, int y)
        {
            Stack<PuzzleState> stack = new Stack<PuzzleState>();
            HashSet<string> visited = new HashSet<string>();

            visited.Add(BoardToString(start));
            PuzzleState initialState = new PuzzleState(start, x, y, 0, visited);
            stack.Push(initialState);
            int iterationCount = 1;

            while (stack.Count > 0)
            {
                PuzzleState current = stack.Pop();

                if (IsGoalState(current.Board))
                {
                    return (iterationCount, GetSolutionPath(current));
                }

                if (current.Depth >= depthLimit)
                {
                    continue;
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
                        if (!current.Visited.Contains(newBoardKey))
                        {
                            var updatedVisited = new HashSet<string>(current.Visited);
                            updatedVisited.Add(newBoardKey);
                            stack.Push(new PuzzleState(newBoard, newX, newY, current.Depth + 1, updatedVisited, current));
                            iterationCount++;
                        }
                    }
                }
            }

            return (iterationCount, new List<int[,]>());
        }
    }
}