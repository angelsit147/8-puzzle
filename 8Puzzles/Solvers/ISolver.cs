using System.Collections.Generic;

namespace _8Puzzles.Solvers
{
    /// <summary>
    /// Інтерфейс для алгоритмів розв’язання головоломки 8-пазлів.
    /// </summary>
    interface ISolver
    {
        /// <summary>
        /// Затримка рендерингу (у мілісекундах) для візуалізації кроків розв’язання.
        /// </summary>
        int RenderDelay { get; }

        /// <summary>
        /// Розв’язує головоломку, повертаючи кількість ітерацій і шлях розв’язку.
        /// </summary>
        /// <param name="start">Початковий стан дошки у вигляді матриці 3x3.</param>
        /// <param name="x">Координата по осі X (стовпець) порожньої клітинки.</param>
        /// <param name="y">Координата по осі Y (рядок) порожньої клітинки.</param>
        /// <returns>Кортеж із кількістю ітерацій алгоритму та списком станів дошки для розв’язку.</returns>
        (int, List<int[,]>) SolvePuzzle(int[,] start, int x, int y);
    }
}