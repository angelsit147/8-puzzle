namespace _8Puzzles.Logging
{
    /// <summary>
    /// Інтерфейс для логування результатів розв’язання головоломки 8-пазлів.
    /// </summary>
    interface ILogger
    {
        /// <summary>
        /// Отримує ім’я файлу для збереження логів.
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Логує результати розв’язання головоломки у файл.
        /// </summary>
        /// <param name="selectedMethod">Назва методу розв’язання (наприклад, LDFS, BFS, IDS).</param>
        /// <param name="computationTime">Час виконання алгоритму в мілісекундах.</param>
        /// <param name="swapsCount">Кількість ходів, виконаних для розв’язання.</param>
        /// <param name="iterationCount">Кількість ітерацій алгоритму.</param>
        /// <param name="initialState">Початковий стан дошки у вигляді матриці 3x3.</param>
        /// <param name="finalState">Кінцевий стан дошки у вигляді матриці 3x3.</param>
        void Log(string selectedMethod, double computationTime, int swapsCount, int iterationCount, int[,] initialState, int[,] finalState);
    }
}