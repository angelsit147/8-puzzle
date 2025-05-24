using System;
using System.IO;
using System.Text;

namespace _8Puzzles.Logging
{
    public class TextLogger : ILogger
    {
        public string FileName { get; }

        // Конструктор: Ініціалізує логер із заданим ім’ям файлу для збереження логів
        public TextLogger(string fileName)
        {
            FileName = fileName;
        }

        // Формує і записує результати автоматичного розв’язання головоломки у файл
        public void Log(string selectedMethod, double computationTime, int swapsAmount, int iterationCount, int[,] initialState, int[,] finalState)
        {
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
