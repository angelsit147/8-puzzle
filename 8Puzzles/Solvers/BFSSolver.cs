using System.Collections.Generic;
using System.Linq;

namespace _8Puzzles.Solvers
{
    /// <summary>
    /// Реалізує алгоритм пошуку в ширину (BFS) для розв’язання головоломки 8-пазлів.
    /// </summary>
    public class BFSSolver : ISolver
    {
        /// <summary>
        /// Розмір дошки гри (3x3).
        /// </summary>
        private const int N = 3;

        /// <summary>
        /// Масив зміщень по рядках для можливих ходів (вліво, вправо, вгору, вниз).
        /// </summary>
        private static readonly int[] row = { 0, 0, -1, 1 };

        /// <summary>
        /// Масив зміщень по стовпцях для можливих ходів (вліво, вправо, вгору, вниз).
        /// </summary>
        private static readonly int[] col = { -1, 1, 0, 0 };

        /// <summary>
        /// Затримка рендерингу (у мілісекундах) для візуалізації кроків розв’язання.
        /// </summary>
        public int RenderDelay { get; }

        /// <summary>
        /// Ініціалізує солвер із заданою затримкою рендерингу.
        /// </summary>
        /// <param name="renderDelay">Затримка рендерингу для відображення кроків (у мілісекундах).</param>
        public BFSSolver(int renderDelay)
        {
            RenderDelay = renderDelay;
        }

        /// <summary>
        /// Перевіряє, чи є поточний стан дошки цільовим (розв’язаним).
        /// </summary>
        /// <param name="board">Матриця 3x3, що представляє поточний стан дошки.</param>
        /// <returns>Повертає true, якщо дошка розв’язана, інакше false.</returns>
        private static bool IsGoalState(int[,] board)
        {
            /// <summary>
            /// Цільовий стан дошки, де числа розташовані від 1 до 8, а 0 — порожня клітинка.
            /// </summary>
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

        /// <summary>
        /// Перевіряє, чи є координати дійсними в межах дошки 3x3.
        /// </summary>
        /// <param name="x">Координата по осі X (стовпець).</param>
        /// <param name="y">Координата по осі Y (рядок).</param>
        /// <returns>Повертає true, якщо координати в межах дошки, інакше false.</returns>
        private static bool IsValid(int x, int y)
        {
            return x >= 0 && x < N && y >= 0 && y < N;
        }

        /// <summary>
        /// Перетворює дошку в рядкове представлення для порівняння станів.
        /// </summary>
        /// <param name="board">Матриця 3x3, що представляє поточний стан дошки.</param>
        /// <returns>Рядок, що унікально ідентифікує стан дошки.</returns>
        private static string BoardToString(int[,] board)
        {
            return string.Join("|", Enumerable.Range(0, N).Select(i =>
                string.Join(",", Enumerable.Range(0, N).Select(j => board[i, j]))));
        }

        /// <summary>
        /// Формує список станів дошки, що ведуть до розв’язку.
        /// </summary>
        /// <param name="goalState">Цільовий стан головоломки, досягнутий алгоритмом.</param>
        /// <returns>Список матриць, що представляють послідовність станів до розв’язку.</returns>
        private static List<int[,]> GetSolutionPath(PuzzleState goalState)
        {
            /// <summary>
            /// Список матриць для збереження шляху розв’язку.
            /// </summary>
            List<int[,]> path = new List<int[,]>();
            /// <summary>
            /// Поточний стан головоломки в процесі побудови шляху.
            /// </summary>
            PuzzleState current = goalState;

            while (current != null)
            {
                path.Add(current.Board);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// Розв’язує головоломку за допомогою алгоритму пошуку в ширину (BFS).
        /// </summary>
        /// <param name="start">Початковий стан дошки у вигляді матриці 3x3.</param>
        /// <param name="x">Координата по осі X (стовпець) порожньої клітинки.</param>
        /// <param name="y">Координата по осі Y (рядок) порожньої клітинки.</param>
        /// <returns>Кортеж із кількістю ітерацій алгоритму та списком станів для розв’язку.</returns>
        public (int, List<int[,]>) SolvePuzzle(int[,] start, int x, int y)
        {
            /// <summary>
            /// Черга для зберігання станів головоломки під час пошуку.
            /// </summary>
            Queue<PuzzleState> queue = new Queue<PuzzleState>();
            /// <summary>
            /// Множина відвіданих станів для уникнення повторного відвідування.
            /// </summary>
            HashSet<string> visited = new HashSet<string>();

            /// <summary>
            /// Початковий стан головоломки для початку пошуку.
            /// </summary>
            PuzzleState initialState = new PuzzleState(start, x, y, 0);
            queue.Enqueue(initialState);
            /// <summary>
            /// Лічильник загальної кількості ітерацій алгоритму.
            /// </summary>
            int totalIterations = 1;
            visited.Add(BoardToString(start));

            while (queue.Count > 0)
            {
                /// <summary>
                /// Поточний стан головоломки, витягнутий із черги.
                /// </summary>
                PuzzleState current = queue.Dequeue();

                if (IsGoalState(current.Board))
                {
                    return (totalIterations, GetSolutionPath(current));
                }

                for (int i = 0; i < 4; i++)
                {
                    /// <summary>
                    /// Нова координата по осі X після можливого ходу.
                    /// </summary>
                    int newX = current.X + row[i];
                    /// <summary>
                    /// Нова координата по осі Y після можливого ходу.
                    /// </summary>
                    int newY = current.Y + col[i];

                    if (IsValid(newX, newY))
                    {
                        /// <summary>
                        /// Новий стан дошки після виконання ходу.
                        /// </summary>
                        int[,] newBoard = (int[,])current.Board.Clone();
                        newBoard[current.X, current.Y] = newBoard[newX, newY];
                        newBoard[newX, newY] = 0;

                        /// <summary>
                        /// Рядковий ключ для нового стану дошки.
                        /// </summary>
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