namespace _8Puzzles.Logging
{
    interface ILogger
    {
        string FileName { get; }

        // Логує результати розв’язання головоломки у файл
        void Log(string selectedMethod, double computationTime, int swapsCount, int iterationCount, int[,] initialState, int[,] finalState);
    }
}
