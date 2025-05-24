using System.Collections.Generic;

namespace _8Puzzles.Solvers
{
    interface ISolver
    {
        int RenderDelay { get; }

        // Розв’язує головоломку, повертаючи кількість ітерацій і шлях розв’язку
        (int, List<int[,]>) SolvePuzzle(int[,] start, int x, int y);
    }
}
