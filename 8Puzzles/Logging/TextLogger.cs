using System;
using System.IO;
using System.Text;

namespace _8Puzzles.Logging
{
    /// <summary>
    /// Реалізує логування результатів розв’язання головоломки 8-пазлів у текстовий файл.
    /// </summary>
    public class TextLogger : ILogger
    {
        /// <summary>
        /// Ім’я файлу для збереження логів результатів розв’язання.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Ініціалізує логер із заданим ім’ям файлу для збереження логів.
        /// </summary>
        /// <param name="fileName">Ім’я файлу, куди записуватимуться логи.</param>
        public TextLogger(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Формує і записує результати автоматичного розв’язання головоломки у текстовий файл.
        /// </summary>
        /// <param name="selectedMethod">Назва методу розв’язання (наприклад, LDFS, BFS, IDS).</param>
        /// <param name="computationTime">Час виконання алгоритму в мілісекундах.</param>
        /// <param name="swapsAmount">Кількість ходів, виконаних для розв’язання.</param>
        /// <param name="iterationCount">Кількість ітерацій алгоритму.</param>
        /// <param name="initialState">Початковий стан дошки у вигляді матриці 3x3.</param>
        /// <param name="finalState">Кінцевий стан дошки у вигляді матриці 3x3.</param>
        public void Log(string selectedMethod, double computationTime, int swapsAmount, int iterationCount, int[,] initialState, int[,] finalState)
        {
            /// <summary>
            /// Буфер для формування текстового представлення результатів розв’язання.
            /// </summary>
            StringBuilder result = new StringBuilder();
            result.AppendLine($"Solve Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            result.AppendLine($"Method: {selectedMethod}");
            result.AppendLine($"Computation Time: {computationTime:F2} ms");
            result.AppendLine($"Number of Swaps: {swapsAmount}");
            result.AppendLine($"Number of Iterations: {iterationCount}");
            result.AppendLine("Initial State:");
            for (int i = 0; i < 3; i++)
            {
                result.AppendLine($"  [{initialState[i, 0],2} {initialState[i, 1],2} {initialState[i, 2],2}]");
            }
            result.AppendLine("Final State:");
            for (int i = 0; i < 3; i++)
            {
                result.AppendLine($"  [{finalState[i, 0],2} {finalState[i, 1],2} {finalState[i, 2],2}]");
            }
            result.AppendLine("--------------------");

            File.AppendAllText(FileName, result.ToString());
        }
    }
}