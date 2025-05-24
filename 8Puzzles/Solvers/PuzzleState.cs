using System.Collections.Generic;

namespace _8Puzzles.Solvers
{
    /// <summary>
    /// Представляє стан головоломки 8-пазлів, включаючи дошку, координати порожньої клітинки та інформацію про пошук.
    /// </summary>
    public class PuzzleState
    {
        /// <summary>
        /// Матриця 3x3, що представляє поточний стан дошки гри.
        /// </summary>
        public int[,] Board { get; }

        /// <summary>
        /// Координата по осі X (стовпець) порожньої клітинки.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Координата по осі Y (рядок) порожньої клітинки.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Глибина поточного стану в дереві пошуку.
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Батьківський стан головоломки, з якого було отримано поточний стан.
        /// </summary>
        public PuzzleState Parent { get; }

        /// <summary>
        /// Множина рядкових ключів відвіданих станів для уникнення циклів у пошуку.
        /// </summary>
        public HashSet<string> Visited { get; }

        /// <summary>
        /// Ініціалізує стан головоломки з дошки, координатами порожньої клітинки, глибиною та батьківським станом.
        /// </summary>
        /// <param name="board">Матриця 3x3, що представляє стан дошки.</param>
        /// <param name="x">Координата по осі X (стовпець) порожньої клітинки.</param>
        /// <param name="y">Координата по осі Y (рядок) порожньої клітинки.</param>
        /// <param name="depth">Глибина поточного стану в дереві пошуку.</param>
        /// <param name="parent">Батьківський стан головоломки (за замовчуванням null).</param>
        public PuzzleState(int[,] board, int x, int y, int depth, PuzzleState parent = null)
        {
            Board = (int[,])board.Clone();
            X = x;
            Y = y;
            Depth = depth;
            Parent = parent;
        }

        /// <summary>
        /// Ініціалізує стан головоломки з дошки, координатами, глибиною, множиною відвіданих станів та батьківським станом.
        /// </summary>
        /// <param name="board">Матриця 3x3, що представляє стан дошки.</param>
        /// <param name="x">Координата по осі X (стовпець) порожньої клітинки.</param>
        /// <param name="y">Координата по осі Y (рядок) порожньої клітинки.</param>
        /// <param name="depth">Глибина поточного стану в дереві пошуку.</param>
        /// <param name="visited">Множина рядкових ключів відвіданих станів.</param>
        /// <param name="parent">Батьківський стан головоломки (за замовчуванням null).</param>
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